import { Component, OnInit, AfterViewChecked, OnDestroy } from "@angular/core";
import { BookingService } from "src/app/_services/booking.service";
import { AlertifyService } from "src/app/_services/alertify.service";
import { formatDate } from "@angular/common";
import { LessonsService } from "src/app/_services/lessons.service";
import { TeachingProgram } from "src/app/_models/teachingprogram";
import { NgxSpinnerService } from "ngx-spinner";
import { Subscription } from "rxjs";
import { Router } from "@angular/router";
import { isThursday } from "date-fns";

declare let paypal: any;

@Component({
  selector: "app-booking",
  templateUrl: "./booking.component.html",
  styleUrls: ["./booking.component.css"],
})
export class BookingComponent implements OnInit, AfterViewChecked, OnDestroy {
  setSubscription: any = {};
  tpAr3: any[] = [];

  temp = new Array();

  lesson: any;
  subscription: any;
  price: any;

  private addScript: boolean = false;
  paypalLoad: boolean = true;

  tpAr: TeachingProgram[] = [];
  tpAr2: TeachingProgram[] = [];

  userid: string;
  resp: string = "";
  lessonsPerMonth: string = "";
  photoUrl: string = "";

  lessonsSub: Subscription;
  bookingSub: Subscription;

  constructor(
    private bookingService: BookingService,
    private lessonsService: LessonsService,
    private alertifyService: AlertifyService,
    private router: Router,
    private alertify: AlertifyService,
    private spinner: NgxSpinnerService
  ) {}

  ngOnDestroy(): void {
    if (this.lessonsSub) this.lessonsSub.unsubscribe();
    if (this.bookingSub) this.bookingSub.unsubscribe();
  }

  ngOnInit(): void {
    this.spinner.show();
    this.initProgram();
    this.lesson = JSON.parse(localStorage.getItem("lesson"));
    this.subscription = JSON.parse(localStorage.getItem("subscription"));
    this.photoUrl = this.lesson.ImageurlLesson;
    console.log(this.lesson);
    console.log(this.subscription);
    this.userid = JSON.parse(localStorage.getItem("decodedToken")).nameid;
    this.price = "00.00";
    this.spinner.hide();
  }

  async initProgram(): Promise<void> {
    await this.getCapacity();
    this.getTeachingHours();
  }

  async getCapacity(): Promise<void> {
    this.lessonsSub = this.lessonsService
      .getCapacity(JSON.parse(localStorage.getItem("lesson")).Lessonid)
      .subscribe(
        (resp: any) => {
          if (resp.Obj !== null && Array.from(resp.Obj).length > 0) {
            resp.Obj.forEach((teachingProgram) => {
              this.tpAr3.push(teachingProgram);
            });
          }
          if (resp.Message == "full") {
            this.resp =
              "All days/hours are full, You cannot buy subscription, feel free to call us!";
          } else {
            if (resp.Message == "not full")
              this.resp = "All days/hours are available to buy a subscription!";
            else this.resp = resp.Message;
          }
        },
        (error) => {
          this.alertifyService.error(error);
        }
      );
  }

  getTeachingHours(): void {
    this.tpAr = [];
    this.lessonsSub = this.lessonsService
      .getTeachingHours(JSON.parse(localStorage.getItem("lesson")).Lessonid)
      .subscribe(
        (teachingProgram: TeachingProgram[]) => {
          teachingProgram.forEach((teachingProgram) => {
            this.tpAr.push({
              Capacity: teachingProgram.Capacity,
              Dayofweek: teachingProgram.Dayofweek,
              Id: teachingProgram.Id,
              Lessonend: teachingProgram.Lessonend,
              Lessonid: teachingProgram.Lessonid,
              Lessonstart: teachingProgram.Lessonstart,
            });
          });
          this.tpAr = this.tpAr.filter(
            (item) => !this.tpAr3.includes(item.Dayofweek)
          );
        },
        (error) => {
          this.alertifyService.error(error);
        }
      );
  }

  paypalConfig = {
    env: "sandbox",
    client: {
      sandbox:
        "AVGk1qriYAZwhcRSJEWHMPfhGS6MluOjn5k8--rLzOTUmrqXuEKg6MNwf_jbptQ7O8Z4R5yidm6xDOBw",
      production: "<your-production-key-here>",
    },
    commit: true,
    payment: (data, actions) => {
      return actions.payment.create({
        payment: {
          transactions: [
            {
              amount: {
                total: parseInt(
                  this.price === "00.00" ? this.price : this.price
                ),
                currency: "EUR",
              },
            },
          ],
        },
      });
    },
    onAuthorize: (data, actions) => {
      return actions.payment.execute().then((payment) => {
        this.registerBooking();
      });
    },
  };

  ngAfterViewChecked(): void {
    if (!this.addScript) {
      this.addPaypalScript().then(() => {
        paypal.Button.render(this.paypalConfig, "#paypal-checkout-btn");
        this.paypalLoad = false;
      });
    }
  }

  addPaypalScript() {
    this.addScript = true;
    return new Promise((resolve, reject) => {
      let scripttagElement = document.createElement("script");
      scripttagElement.src = "https://www.paypalobjects.com/api/checkout.js";
      scripttagElement.onload = resolve;
      document.body.appendChild(scripttagElement);
    });
  }

  registerBooking(): void {
    this.spinner.show();
    if (this.tpAr2.length > 0) {
      let tpDaysTemp: string[];
      this.setSubscription.Userid = this.userid;
      this.setSubscription.Lessonid = JSON.parse(
        localStorage.getItem("lesson")
      ).Lessonid;
      this.setSubscription.Duration = JSON.parse(
        localStorage.getItem("subscription")
      ).Duration;
      this.setSubscription.Price = this.price;
      tpDaysTemp = [];
      this.tpAr2.forEach((tp) => {
        tpDaysTemp.push(tp.Dayofweek);
      });
      this.setSubscription.Day = tpDaysTemp;
      this.setSubscription.Discount = JSON.parse(
        localStorage.getItem("subscription")
      ).Discount;
      this.setSubscription.Date = formatDate(
        new Date(new Date()),
        "yyyy/MM/dd",
        "en"
      );
      this.bookingSub = this.bookingService
        .registerBooking(this.setSubscription)
        .subscribe(
          (res) => {
            this.spinner.hide();
            this.alertify.success(res.Message);
            this.navigateLessons();
          },
          (error) => {
            this.spinner.hide();
            this.alertify.error(error);
          }
        );
    } else {
      this.spinner.hide();
      this.alertify.error("Choose Day first!");
    }
  }

  private navigateLessons(): void {
    setTimeout(() => {
      this.router.navigate(["/lessons"]);
    }, 1500);
  }

  showOptions(event, temp: TeachingProgram): void {
    if (event.checked === true) {
      if (!this.tpAr2.find((x) => x.Id === temp.Id)) {
        this.tpAr2.push(temp);
      }
    } else {
      this.tpAr2 = Array.from(this.tpAr2).filter((x) => x.Id !== temp.Id);
    }

    if (
      this.lesson.Discountprice != "" &&
      this.lesson.Discountprice != null &&
      this.lesson.Discountprice &&
      parseFloat(this.lesson.Discountprice) > 0 &&
      this.tpAr2.length === this.tpAr.length
    ) {
      this.price = this.lesson.Discountprice * this.tpAr2.length;
    } else {
      this.price = this.lesson.Price * this.tpAr2.length;
    }
    
    this.price = Number(this.price).toFixed(2);

    if (this.tpAr2.length > 0) {
      this.lessonsPerMonth = (this.tpAr2.length * 4).toString();
    } else {
      this.lessonsPerMonth = "";
    }
  }
}
