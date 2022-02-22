import { Component, OnDestroy, OnInit } from "@angular/core";
import { AlertifyService } from "src/app/_services/alertify.service";
import { LessonsService } from "src/app/_services/lessons.service";
import { ActivatedRoute, Router } from "@angular/router";
import { UtubeModalComponent } from "../utube-modal/utube-modal.component";
import { CustomLesson } from "src/app/_models/customlesson";
import { TeachingProgram } from "src/app/_models/teachingprogram";
import { Subscriptions } from "src/app/_models/subscriptions";
import { TeachingprogramService } from "src/app/_services/teachingprogram.service";
import { NgxSpinnerService } from "ngx-spinner";
import { Subscription } from "rxjs";
import { MatDialog } from "@angular/material";

@Component({
  selector: "app-lesson-detail",
  templateUrl: "./lesson-detail.component.html",
  styleUrls: ["./lesson-detail.component.css"],
})
export class LessonDetailComponent implements OnInit, OnDestroy {
  model: any = {};

  message: string;

  visible: boolean = false;

  customLesson: CustomLesson;
  teachingProgram: TeachingProgram;

  subscriptions: Subscriptions[];
  tpAr: TeachingProgram[] = [];
  customLessonAr: CustomLesson[] = [];
  subsAr: Subscriptions[] = [];

  teachingProgramSub: Subscription;
  lessonsSub: Subscription;

  constructor(
    private alertifyService: AlertifyService,
    private lessonService: LessonsService,
    private route: ActivatedRoute,
    private dialog: MatDialog,
    private teachingprogramService: TeachingprogramService,
    private router: Router,
    private spinner: NgxSpinnerService
  ) {}

  ngOnDestroy(): void {
    if (this.teachingProgramSub) this.teachingProgramSub.unsubscribe();
    if (this.lessonsSub) this.lessonsSub.unsubscribe();
  }

  ngOnInit(): void {
    this.spinner.show();
    this.getCapacity();
    this.getCourse();
    this.getSubscriptions();
    this.getTeachingHours();
    this.spinner.hide();
  }

  registerClassroom(tp: TeachingProgram): void {
    this.spinner.show();
    this.model.Capacity = tp.Capacity;
    this.model.Day = tp.Dayofweek;
    this.model.Lessonstart = tp.Lessonstart;
    this.model.Lessonsend = tp.Lessonend;
    this.model.Lessonid = tp.Lessonid;
    this.model.Userid = JSON.parse(localStorage.getItem("decodedToken")).nameid;

    this.teachingProgramSub = this.teachingprogramService
      .registerClassroom(this.model)
      .subscribe(
        (res) => {
          this.spinner.hide();
          this.alertifyService.success(res.Message);
        },
        (error) => {
          this.spinner.hide();
          this.alertifyService.error(error);
        }
      );
  }

  getCapacity(): void {
    this.lessonsSub = this.lessonService
      .getCapacity(this.route.snapshot.params["id"].toString())
      .subscribe(
        (resp: any) => {
          if (resp == "full") this.visible = true;
          else this.visible = false;
        },
        (error) => {
          this.alertifyService.error(error);
        }
      );
  }

  getCourse(): void {
    this.customLessonAr = [];
    this.lessonsSub = this.lessonService
      .getLessonCustom(this.route.snapshot.params["id"].toString())
      .subscribe(
        (customLesson: CustomLesson) => {
          this.customLessonAr.push({
            DescrLesson: customLesson.DescrLesson,
            DescrTeacher: customLesson.DescrTeacher,
            Discountprice: customLesson.Discountprice,
            Firstname: customLesson.Firstname,
            Lastname: customLesson.Lastname,
            ImageurlLesson: customLesson.ImageurlLesson,
            ImageurlTeacher: customLesson.ImageurlTeacher,
            Lesson: customLesson.Lesson,
            Lessonid: customLesson.Lessonid,
            Teacherid: customLesson.Teacherid,
            Utubeurl: customLesson.Utubeurl,
            Discount: customLesson.Discount,
            Price: customLesson.Price,
          });
          localStorage.setItem(
            "lesson",
            JSON.stringify(this.customLessonAr[0])
          );
        },
        (error) => {
          this.alertifyService.error(error);
        }
      );
  }

  getTeachingHours(): void {
    this.tpAr = [];
    this.lessonsSub = this.lessonService
      .getTeachingHours(this.route.snapshot.params["id"].toString())
      .subscribe((teachingProgram: TeachingProgram[]) => {
        teachingProgram.forEach(
          (teachingProgram) => {
            this.tpAr.push({
              Capacity: teachingProgram.Capacity,
              Dayofweek: teachingProgram.Dayofweek,
              Id: teachingProgram.Id,
              Lessonend: teachingProgram.Lessonend,
              Lessonid: teachingProgram.Lessonid,
              Lessonstart: teachingProgram.Lessonstart,
            });
          },
          (error) => {
            this.alertifyService.error(error);
          }
        );
      });
  }

  getSubscriptions(): void {
    this.subsAr = [];
    this.lessonsSub = this.lessonService
      .getSubscriptions(this.route.snapshot.params["id"].toString())
      .subscribe(
        (subscriptions: Subscriptions[]) => {
          subscriptions.forEach((subscriptions) => {
            this.subsAr.push({
              Discount: subscriptions.Discount,
              Duration: subscriptions.Duration,
              Discprice: subscriptions.Discprice,
              Id: subscriptions.Id,
              Lessonid: subscriptions.Lessonid,
              Price: subscriptions.Price,
            });
          });
        },
        (error) => {
          this.alertifyService.error(error);
        }
      );
  }

  subscribe(subscription: Subscriptions): void {
    if (this.visible == true)
      this.alertifyService.error(
        "You cannot proceed to buy subscription. All departments are full!"
      );
    else {
      localStorage.setItem("subscription", JSON.stringify(subscription));
      localStorage.setItem("lesson", JSON.stringify(this.customLessonAr[0]));
      this.router.navigate(["/payment/"]);
    }
  }

  openYoutubeModal(): void {
    const dialogRef = this.dialog.open(UtubeModalComponent);

    dialogRef.afterClosed().subscribe((result) => {
      console.log(result);
    });
  }
}
