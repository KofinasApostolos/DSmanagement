import {
  Component,
  OnInit,
  ViewChild,
  ChangeDetectorRef,
  OnDestroy,
  QueryList,
  ViewChildren,
} from "@angular/core";
import {
  MatPaginator,
  MatTableDataSource,
  MatSort,
  MatTable,
} from "@angular/material";
import { AlertifyService } from "src/app/_services/alertify.service";
import {
  animate,
  state,
  style,
  transition,
  trigger,
} from "@angular/animations";
import { Observable, Subscription } from "rxjs";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { environment } from "src/environments/environment";
import { TeachingprogramService } from "src/app/_services/teachingprogram.service";
import { TeachingProgramTemp } from "src/app/_models/teachingprogramtemp";
import { DaysService } from "src/app/_services/days.service";
import { LessonsService } from "src/app/_services/lessons.service";
import { Lessons } from "src/app/_models/lessons";
import { NgxSpinnerService } from "ngx-spinner";

@Component({
  selector: "app-usersuggestion-management",
  templateUrl: "./usersuggestion-management.component.html",
  styleUrls: ["./usersuggestion-management.component.css"],
  animations: [
    trigger("detailExpand", [
      state("collapsed", style({ height: "0px", minHeight: "0" })),
      state("expanded", style({ height: "*" })),
      transition(
        "expanded <=> collapsed",
        animate("225ms cubic-bezier(0.4, 0.0, 0.2, 1)")
      ),
    ]),
  ],
})
export class UsersuggestionManagementComponent implements OnInit, OnDestroy {
  private model: any = {};
  day: any[];

  lessonsData: Lesson[] = [];
  lesson: Lessons[];
  teachingprogramtemp: TeachingProgramTemp[];

  expandedElement: Lesson | null;

  private baseUrl = environment.apiUrl;

  dataSource: MatTableDataSource<Lesson>;

  columnsToDisplay = [
    "lessonid",
    "dayofweek",
    "lessonstart",
    "lessonend",
    "Count",
    "actionsColumn",
  ];

  innerDisplayedColumns = ["Name", "Email", "Phone"];

  teachingProgramTempSub: Subscription;
  daysSub: Subscription;
  lessonsSub: Subscription;

  @ViewChild("outerSort") sort: MatSort;
  @ViewChildren("innerSort") innerSort: QueryList<MatSort>;
  @ViewChildren("innerTables") innerTables: QueryList<MatTable<Users>>;
  @ViewChild(MatPaginator) paginator: MatPaginator;

  constructor(
    private cd: ChangeDetectorRef,
    private alertify: AlertifyService,
    private http: HttpClient,
    private daysService: DaysService,
    private lessonService: LessonsService,
    private teachingProgramService: TeachingprogramService,
    private spinner: NgxSpinnerService
  ) {}

  ngOnDestroy(): void {
    if (this.teachingProgramTempSub) this.teachingProgramTempSub.unsubscribe();
    if (this.daysSub) this.daysSub.unsubscribe();
    if (this.lessonsSub) this.lessonsSub.unsubscribe();
  }

  ngOnInit(): void {
    this.spinner.show();
    this.getLessons();
    this.getDays();
    this.teachingProgramTempSub = this.getTeachingProgramsTempAdmin().subscribe(
      (teachingprogramtemp: Lesson[]) => {
        teachingprogramtemp.forEach((lesson) => {
          if (
            lesson.users &&
            Array.isArray(lesson.users) &&
            lesson.users.length
          ) {
            lesson.count = lesson.users.length;
            this.lessonsData = [
              ...this.lessonsData,
              { ...lesson, users: new MatTableDataSource(lesson.users) },
            ];
          } else {
            this.lessonsData = [...this.lessonsData, lesson];
          }
        });
        this.dataSource = new MatTableDataSource(this.lessonsData);
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

  getDays(): void {
    this.daysSub = this.daysService.getDaysInt().subscribe(
      (day: any[]) => {
        this.day = day;
      },
      (error) => {
        this.alertify.error(error);
      }
    );
  }

  applyFilter1(filterValue: string): void {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  getLessons(): void {
    this.lessonsSub = this.lessonService.getLessons().subscribe(
      (lesson: Lessons[]) => {
        this.lesson = lesson;
      },
      (error) => {
        this.alertify.error(error);
      }
    );
  }

  getTeachingProgramsTempAdmin(): Observable<Lesson[]> {
    var headers = new HttpHeaders();
    headers = headers.append(
      "Authorization",
      "Bearer " + localStorage.getItem("ecryptedToken")
    );
    headers = headers.append("Content-Type", "application/json");
    return this.http.get<Lesson[]>(
      this.baseUrl + "teachingprogram/getsuggestionadmin",
      {
        headers: headers,
      }
    );
  }

  toggleRow(element: Lesson): void {
    element.users && (element.users as MatTableDataSource<Users>).data.length
      ? (this.expandedElement =
          this.expandedElement === element ? null : element)
      : null;
    this.cd.detectChanges();
    this.innerTables.forEach(
      (table, index) =>
        ((table.dataSource as MatTableDataSource<Users>).sort =
          this.innerSort.toArray()[index])
    );
  }

  applyFilter(filterValue: string): void {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
    this.innerTables.forEach(
      (table, index) =>
        ((table.dataSource as MatTableDataSource<Users>).filter = filterValue
          .trim()
          .toLowerCase())
    );
  }

  async transferToCore(
    lessonid: string,
    dayofweek: string,
    lessonstart: string,
    lessonend: string
  ): Promise<void> {
    await this.alertify
      .openConfirmDialog(
        "Are you sure you want to transfer this record",
        "Transfer"
      )
      .then((res) => {
        if (res.value) {
          this.spinner.show();
          this.model.Lessonid = lessonid;
          this.model.Dayofweek = dayofweek;
          this.model.Lessonstart = lessonstart;
          this.model.Lessonend = lessonend;
          this.teachingProgramTempSub = this.teachingProgramService
            .transferToCoreProgram(this.model)
            .subscribe(
              (res) => {
                this.spinner.hide();
                this.alertify.success(res.Message);
              },
              (error) => {
                this.spinner.hide();
                this.alertify.error(error);
              }
            );
        }
      });
  }
}

export interface Lesson {
  lessonid: string;
  dayofweek: number;
  lessonstart: string;
  lessonend: string;
  count: number;
  users?: Users[] | MatTableDataSource<Users>;
}

export interface Users {
  fullname: string;
  email: string;
  phone: string;
}
