import {
  Component,
  OnInit,
  ViewChild,
  ChangeDetectorRef,
  OnDestroy,
} from "@angular/core";
import { Lessons } from "../_models/lessons";
import { TeachingProgramTemp } from "../_models/teachingprogramtemp";
import { AlertifyService } from "../_services/alertify.service";
import { DaysService } from "../_services/days.service";
import { LessonsService } from "../_services/lessons.service";
import { TeachingprogramService } from "../_services/teachingprogram.service";
import { MatPaginator, MatTableDataSource, MatSort } from "@angular/material";
import { NgxSpinnerService } from "ngx-spinner";
import { Subscription } from "rxjs";
import { Helpers } from "../_helpers/helper";

@Component({
  selector: "app-user-suggestion",
  templateUrl: "./user-suggestion.component.html",
  styleUrls: ["./user-suggestion.component.css"],
})
export class UserSuggestionComponent implements OnInit, OnDestroy {
  model: any = {};
  day: any[];

  private lesson: Lessons[];
  teachingprogramtemp: TeachingProgramTemp[];

  dataSource: MatTableDataSource<TeachingProgramTemp>;

  displayedColumns = [
    "Id",
    "Lesson",
    "Dayofweek",
    "Lessonstart",
    "Lessonend",
    "Count",
    "actionsColumn",
  ];

  teachingProgramSub: Subscription;
  daysSub: Subscription;
  lessonsSub: Subscription;

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  constructor(
    private lessonService: LessonsService,
    private dayService: DaysService,
    private alertifyService: AlertifyService,
    private alertify: AlertifyService,
    private teachingprogramService: TeachingprogramService,
    private changeDetectorRefs: ChangeDetectorRef,
    private spinner: NgxSpinnerService
  ) {}

  ngOnDestroy(): void {
    if (this.teachingProgramSub) this.teachingProgramSub.unsubscribe();
    if (this.daysSub) this.daysSub.unsubscribe();
    if (this.lessonsSub) this.lessonsSub.unsubscribe();
  }

  ngOnInit(): void {
    this.initDays();
    this.spinner.show();
    this.getSuggestions();
    this.getLessons();
    this.spinner.hide();
  }

  async initDays(): Promise<void> {
    await this.getDays();
  }

  refresh(): void {
    this.teachingProgramSub = this.teachingprogramService
      .getTeachingProgramsTempUser(
        JSON.parse(localStorage.getItem("decodedToken")).nameid
      )
      .subscribe((teachingprogramtemp: TeachingProgramTemp[]) => {
        this.teachingprogramtemp = teachingprogramtemp;
        this.dataSource = new MatTableDataSource(this.teachingprogramtemp);
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
        this.changeDetectorRefs.detectChanges();
      });
  }

  getErrorMessage(controlName: string, errorType: string) {
    return Helpers.getErrorMessageTemplateDrivenForm(controlName, errorType);
  }

  async deleteSuggestion(
    lessonid: number,
    id: string,
    day: string,
    lessonstart: string,
    lessonend: string,
    count: string
  ): Promise<void> {
    await this.alertify
      .openConfirmDialog(
        "Are you sure you want to delete this record?",
        "Delete"
      )
      .then((res) => {
        if (res.value) {
          this.spinner.show();
          this.model.Lessonid = lessonid;
          this.model.Id = id;
          this.model.Day = day;
          this.model.Lessonstart = lessonstart;
          this.model.Lessonend = lessonend;
          this.model.Count = count;
          this.teachingProgramSub = this.teachingprogramService
            .deleteSuggestionUser(
              this.model,
              JSON.parse(localStorage.getItem("decodedToken")).nameid
            )
            .subscribe(
              () => {
                this.alertifyService.success("Delete has been successful");
                this.refresh();
                this.spinner.hide();
              },
              (error) => {
                this.spinner.hide();
                this.alertify.error(error);
              }
            );
        }
      });
  }

  async updateSuggestion(
    id: string,
    lessonid: string,
    day: string,
    lessonstart: string,
    lessonend: string
  ): Promise<void> {
    await this.alertify
      .openConfirmDialog(
        "Are you sure you want to update this record",
        "Update"
      )
      .then((res) => {
        if (res.value) {
          this.spinner.show();
          this.model.Id = id;
          this.model.Lessonid = lessonid;
          this.model.Dayofweek = day;
          this.model.Lessonstart = lessonstart;
          this.model.Lessonend = lessonend;
          this.teachingProgramSub = this.teachingprogramService
            .updateSuggestionUser(
              JSON.parse(localStorage.getItem("decodedToken")).nameid,
              this.model
            )
            .subscribe(
              (res) => {
                // this.teachingprogramtemp = TeachingProgramTemp;
                this.refresh();
                this.alertify.success(res.Message);
                this.spinner.hide();
              },
              (error) => {
                this.spinner.hide();
                this.alertify.error(error);
              }
            );
        }
      });
  }

  submitSuggestion(): void {
    this.spinner.show();
    this.model.Userid = JSON.parse(localStorage.getItem("decodedToken")).nameid;
    this.teachingProgramSub = this.teachingprogramService
      .submitSuggestion(this.model)
      .subscribe(
        () => {
          this.refresh();
          this.alertifyService.success("suggestion submitted successfully");
          this.spinner.hide();
        },
        (error) => {
          this.spinner.hide();
          this.alertifyService.error(error);
        }
      );
  }

  async getDays(): Promise<void> {
    this.daysSub = this.dayService.getDaysInt().subscribe(
      (day: any[]) => {
        this.day = day;
      },
      (error) => {
        this.alertifyService.error(error);
      }
    );
  }

  getSuggestions(): void {
    this.teachingProgramSub = this.teachingprogramService
      .getTeachingProgramsTempUser(
        JSON.parse(localStorage.getItem("decodedToken")).nameid
      )
      .subscribe(
        (teachingprogramtemp: TeachingProgramTemp[]) => {
          this.teachingprogramtemp = teachingprogramtemp;
          this.dataSource = new MatTableDataSource(this.teachingprogramtemp);
          this.dataSource.paginator = this.paginator;
          this.dataSource.sort = this.sort;
        },
        (error) => {
          this.alertifyService.error(error);
        }
      );
  }

  getLessons(): void {
    this.lessonsSub = this.lessonService.getLessons().subscribe(
      (lesson: Lessons[]) => {
        this.lesson = lesson;
      },
      (error) => {
        this.alertifyService.error(error);
      }
    );
  }
}
