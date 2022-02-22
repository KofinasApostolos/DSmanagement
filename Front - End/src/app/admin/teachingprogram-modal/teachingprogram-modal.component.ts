import { Component, OnDestroy, OnInit } from "@angular/core";
import { AlertifyService } from "src/app/_services/alertify.service";
import { LessonsService } from "src/app/_services/lessons.service";
import { TeachingprogramService } from "src/app/_services/teachingprogram.service";
import { Lessons } from "src/app/_models/lessons";
import { TeachingProgram } from "src/app/_models/teachingprogram";
import { DaysService } from "src/app/_services/days.service";
import { Subject, Subscription } from "rxjs";
import { Helpers } from "src/app/_helpers/helper";

@Component({
  selector: "app-teachingprogram-modal",
  templateUrl: "./teachingprogram-modal.component.html",
  styleUrls: ["./teachingprogram-modal.component.css"],
})
export class TeachingprogramModalComponent implements OnInit, OnDestroy {
  model: any = {};
  day: any[];

  lessons: Lessons[];
  teachingprogram: TeachingProgram[];

  public onClose: Subject<boolean>;

  daysSub: Subscription;
  teachingProgramSub: Subscription;

  constructor(
   //public bsModalRef: BsModalRef,
    private lessonsService: LessonsService,
    private daysService: DaysService,
    private TeachingProgramService: TeachingprogramService,
    private alertify: AlertifyService
  ) {}

  ngOnDestroy(): void {
    if (this.daysSub) this.daysSub.unsubscribe();
    if (this.teachingProgramSub) this.teachingProgramSub.unsubscribe();
  }

  ngOnInit(): void {
    this.onClose = new Subject();
    this.getLessons();
    this.getDays();
  }

  getLessons(): void {
    this.daysSub = this.lessonsService.getLessons().subscribe(
      (lessons: Lessons[]) => {
        this.lessons = lessons;
      },
      (error) => {
        this.alertify.error(error);
      }
    );
  }

  public onConfirm(): void {
    this.teachingProgramSub =
      this.TeachingProgramService.registerTeachingProgram(this.model).subscribe(
        (res) => {
          this.onClose.next(true);
          this.alertify.success(res.Message);
        },
        (error) => {
          this.alertify.error(error);
        }
      );
  }

  getErrorMessage(controlName: string, errorType: string) {
    return Helpers.getErrorMessageTemplateDrivenForm(controlName, errorType);
  }

  public onCancel(): void {
    this.onClose.next(false);
  }

  getDays(): void {
    this.daysSub = this.daysService.getDays().subscribe(
      (day: any[]) => {
        this.day = day;
      },
      (error) => {
        this.alertify.error(error);
      }
    );
  }
}
