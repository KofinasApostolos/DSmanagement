import { Component, OnDestroy, OnInit } from "@angular/core";
import { AlertifyService } from "src/app/_services/alertify.service";
import { LessonsService } from "src/app/_services/lessons.service";
import { Lessons } from "src/app/_models/lessons";
import { SubscriptionsService } from "src/app/_services/subscriptions.service";
import { Subject, Subscription } from "rxjs";
import { Helpers } from "src/app/_helpers/helper";

@Component({
  selector: "app-subscription-modal",
  templateUrl: "./subscription-modal.component.html",
  styleUrls: ["./subscription-modal.component.css"],
})
export class SubscriptionModalComponent implements OnInit, OnDestroy {
  model: any = {};

  lessons: Lessons[];

  check: boolean;
  discount: boolean = true;

  public onClose: Subject<boolean>;

  subscriptionSub: Subscription;
  lessonsSub: Subscription;

  constructor(
    // public bsModalRef: BsModalRef,
    private lessonService: LessonsService,
    private subscriptionService: SubscriptionsService,
    private alertify: AlertifyService
  ) {}

  ngOnDestroy(): void {
    if (this.subscriptionSub) this.subscriptionSub.unsubscribe();
    if (this.lessonsSub) this.lessonsSub.unsubscribe();
  }

  ngOnInit(): void {
    this.getLessons();
    this.onClose = new Subject();
  }

  public onConfirm(): void {
    this.subscriptionSub = this.subscriptionService
      .registerSubscription(this.model)
      .subscribe(
        (res) => {
          this.onClose.next(true);
          this.alertify.success(res.Message);
        },
        (error) => {
          this.alertify.error(error);
        }
      );
  }

  public onCancel(): void {
    this.onClose.next(false);
  }

  getLessons(): void {
    this.lessonsSub = this.lessonService.getLessons().subscribe(
      (lessons: Lessons[]) => {
        this.lessons = lessons;
      },
      (error) => {
        this.alertify.error(error);
      }
    );
  }

  getErrorMessage(controlName: string, errorType: string) {
    return Helpers.getErrorMessageTemplateDrivenForm(controlName, errorType);
  }

  showOptions(event): void {
    this.check = event.checked;
    if (this.check) {
      this.discount = false;
    } else {
      this.discount = true;
    }
  }
}
