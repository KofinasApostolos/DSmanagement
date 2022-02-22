import { Component, OnInit, Input, OnDestroy } from "@angular/core";
import { Lessons } from "src/app/_models/lessons";
import { LessonsService } from "src/app/_services/lessons.service";
import { Router } from "@angular/router";
import { AlertifyService } from "src/app/_services/alertify.service";
import { Subscription } from "rxjs";

@Component({
  selector: "app-lesson-card",
  templateUrl: "./lesson-card.component.html",
  styleUrls: ["./lesson-card.component.css"],
})
export class LessonCardComponent implements OnInit, OnDestroy {
  @Input() lessons: Lessons;

  lessonsSub: Subscription;

  constructor(
    private lessonService: LessonsService,
    private route: Router,
    private alertify: AlertifyService
  ) {}

  ngOnDestroy(): void {
    if (this.lessonsSub) this.lessonsSub.unsubscribe();
  }

  ngOnInit(): void {}

  checkLesson(lessonid: string): void {
    this.lessonsSub = this.lessonService.getLessonCustom(lessonid).subscribe(
      (data) => this.route.navigate(["/lesson/" + lessonid]),
      (err) => this.alertify.error(err),
      () => {}
    );
  }
}
