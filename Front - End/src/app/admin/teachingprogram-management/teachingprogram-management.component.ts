import {
  Component,
  OnInit,
  ViewChild,
  ChangeDetectorRef,
  OnDestroy,
} from "@angular/core";
import {
  MatPaginator,
  MatTableDataSource,
  MatSort,
  MatDialog,
} from "@angular/material";
import { AlertifyService } from "src/app/_services/alertify.service";
import { Lessons } from "src/app/_models/lessons";
import { LessonsService } from "src/app/_services/lessons.service";
import { TeachingProgram } from "src/app/_models/teachingprogram";
import { TeachingprogramService } from "src/app/_services/teachingprogram.service";
import { TeachingprogramModalComponent } from "../teachingprogram-modal/teachingprogram-modal.component";
import { DaysService } from "src/app/_services/days.service";
import { NgxSpinnerService } from "ngx-spinner";
import * as fileSaver from "file-saver";
import { Subscription } from "rxjs";
import { Helpers } from "src/app/_helpers/helper";

@Component({
  selector: "app-teachingprogram-management",
  templateUrl: "./teachingprogram-management.component.html",
  styleUrls: ["./teachingprogram-management.component.css"],
})
export class TeachingprogramManagementComponent implements OnInit, OnDestroy {
  model: any = {};

  lesson: Lessons[];
  day: any[];
  teachingprogram: TeachingProgram[];

  //private bsModalRef: BsModalRef;

  dataSource: MatTableDataSource<TeachingProgram>;

  displayedColumns = [
    "Id",
    "Lesson",
    "Dayofweek",
    "Lessonstart",
    "Lessonend",
    "Capacity",
    "actionsColumn",
  ];

  teachingProgramSub: Subscription;
  daysSub: Subscription;
  lessonsSub: Subscription;

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  constructor(
    private alertify: AlertifyService,
    private lessonsService: LessonsService,
    private dialog: MatDialog,
    private daysService: DaysService,
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
    this.getLessons();
    this.getTeachingPrograms();
  }

  async initDays(): Promise<void> {
    await this.getDays();
  }

  refresh(): void {
    this.teachingProgramSub = this.teachingprogramService
      .getTeachingPrograms()
      .subscribe((teachingprogram: TeachingProgram[]) => {
        this.teachingprogram = teachingprogram;
        this.dataSource = new MatTableDataSource(this.teachingprogram);
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
        this.changeDetectorRefs.detectChanges();
      });
  }

  async getDays(): Promise<void> {
    this.daysSub = this.daysService.getDaysInt().subscribe(
      (day: any[]) => {
        this.day = day;
      },
      (error) => {
        this.alertify.error(error);
      }
    );
  }

  getTeachingPrograms(): void {
    this.spinner.show();
    this.teachingProgramSub = this.teachingprogramService
      .getTeachingPrograms()
      .subscribe(
        (teachingprogram: TeachingProgram[]) => {
          this.teachingprogram = teachingprogram;
          this.dataSource = new MatTableDataSource(this.teachingprogram);
          this.dataSource.paginator = this.paginator;
          this.dataSource.sort = this.sort;
          this.spinner.hide();
        },
        (error) => {
          this.spinner.hide();
          this.alertify.error(error);
        }
      );
  }

  getLessons(): void {
    this.lessonsSub = this.lessonsService.getLessons().subscribe(
      (lesson: Lessons[]) => {
        this.lesson = lesson;
      },
      (error) => {
        this.alertify.error(error);
      }
    );
  }

  applyFilter(filterValue: string): void {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  async deleteTeachingProgram(Id: number): Promise<void> {
    await this.alertify
      .openConfirmDialog(
        "Are you sure you want to delete this record?",
        "Delete"
      )
      .then((res) => {
        if (res.value) {
          this.spinner.show();
          this.teachingProgramSub = this.teachingprogramService
            .deleteTeachingProgram(Id)
            .subscribe(
              (res) => {
                this.alertify.success(res.Message);
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

  exportXLSTeachingProgram(): void {
    if (this.teachingprogram != null) {
      this.spinner.show();
      this.teachingProgramSub = this.teachingprogramService
        .getTeachingProgramXLS(
          JSON.parse(localStorage.getItem("decodedToken")).nameid,
          this.teachingprogram
        )
        .subscribe(
          (response: any) => {
            if (response) {
              var blob = Helpers.base64ToBlob(response.Obj, "text/plain");
              fileSaver.saveAs(blob, "teaching_program.xlsx");
            }
            this.alertify.success(response.Message);
            this.spinner.hide();
          },
          (error) => {
            this.spinner.hide();
            this.alertify.error(error);
          }
        );
    }
  }

  createTeachingProgram(teachingprogram: TeachingProgram): void {
    const initialState = {
      teachingprogram,
    };
    const dialogRef = this.dialog.open(TeachingprogramModalComponent, {
      data: { initialState },
    });
    dialogRef.afterClosed().subscribe((result) => {
      if (result === true) {
        console.log(result);
        this.spinner.show();
        this.refresh();
        this.spinner.hide();
      }
    });
  }

  async updateTeachingProgram(
    Id: string,
    Lessonid: string,
    Dayofweek: string,
    Lessonstart: string,
    Lessonend: string,
    Capacity: string
  ): Promise<void> {
    await this.alertify
      .openConfirmDialog(
        "Are you sure you want to update this record",
        "Update"
      )
      .then((res) => {
        if (res.value) {
          this.spinner.show();
          this.model.Id = Id;
          this.model.Lessonid = Lessonid;
          this.model.Dayofweek = Dayofweek;
          this.model.Lessonstart = Lessonstart;
          this.model.Lessonend = Lessonend;
          this.model.Capacity = Capacity;
          this.teachingProgramSub = this.teachingprogramService
            .updateTeachingProgram(this.model)
            .subscribe(
              (res) => {
                this.alertify.success(res.Message);
                this.refresh();
                this.spinner.hide();
              },
              (error) => {
                this.alertify.error(error);
                this.spinner.hide();
              }
            );
        }
      });
  }
}
