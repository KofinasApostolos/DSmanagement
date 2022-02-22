import { Component, OnDestroy, OnInit } from "@angular/core";
import { Lessons } from "../_models/lessons";
import { LessonsService } from "../_services/lessons.service";
import { AlertifyService } from "../_services/alertify.service";
import { NgxSpinnerService } from "ngx-spinner";
import { Subscription } from "rxjs";

@Component({
  selector: "app-lessons",
  templateUrl: "./lessons.component.html",
  styleUrls: ["./lessons.component.css"],
})
export class LessonsComponent implements OnInit, OnDestroy {
  model: any = {};

  lessons: Lessons[];

  lessonSub: Subscription;

  constructor(
    private lessonsService: LessonsService,
    private alertify: AlertifyService,
    private spinner: NgxSpinnerService
  ) {}

  ngOnDestroy(): void {
    if (this.lessonSub) this.lessonSub.unsubscribe();
  }

  ngOnInit(): void {
    this.loadLessons();
  }

  loadLessons(): void {
    this.spinner.show();
    this.lessonSub = this.lessonsService.getLessons().subscribe(
      (lessons: Lessons[]) => {
        this.lessons = lessons;
        this.spinner.hide();
      },
      (error) => {
        this.spinner.hide();
        this.alertify.error(error);
      }
    );
  }
}
