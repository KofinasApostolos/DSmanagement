import {
  Component,
  OnInit,
  ViewChild,
  ChangeDetectorRef,
  OnDestroy,
  EventEmitter,
  Output,
} from "@angular/core";
import { MatPaginator, MatTableDataSource, MatSort } from "@angular/material";
import { AlertifyService } from "src/app/_services/alertify.service";
import { MatDialog } from "@angular/material/dialog";
import { Lessons } from "src/app/_models/lessons";
import { LessonsService } from "src/app/_services/lessons.service";
import { LessonModalComponent } from "../lesson-modal/lesson-modal.component";
import { Teachers } from "src/app/_models/teachers";
import { HttpClient, HttpEventType, HttpHeaders } from "@angular/common/http";
import { environment } from "src/environments/environment";
import { UserService } from "src/app/_services/user.service";
import { NgxSpinnerService } from "ngx-spinner";
import { Subscription } from "rxjs";

@Component({
  selector: "app-lesson-management",
  templateUrl: "./lesson-management.component.html",
  styleUrls: ["./lesson-management.component.css"],
})
export class LessonManagementComponent implements OnInit, OnDestroy {
  model: any = {};

  private lessons: Lessons[];
  private teachers: Teachers[];

  dataSource: MatTableDataSource<Lessons>;

  private baseUrl = environment.apiUrl;

  filename: string;
  lessonid: string;

  lessonsSub: Subscription;
  teachersSub: Subscription;

  displayedColumns = [
    "Lessonid",
    "Image",
    "Lesson",
    "Teacherid",
    "Description",
    "Youtube",
    "actionsColumn",
  ];

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  @Output() public onUploadFinished = new EventEmitter();

  constructor(
    private alertify: AlertifyService,
    private dialog: MatDialog,
    private lessonsService: LessonsService,
    private userService: UserService,
    private changeDetectorRefs: ChangeDetectorRef,
    private http: HttpClient,
    private spinner: NgxSpinnerService
  ) {}

  ngOnDestroy(): void {
    if (this.lessonsSub) this.lessonsSub.unsubscribe();
    if (this.teachersSub) this.teachersSub.unsubscribe();
  }

  ngOnInit(): void {
    this.getTeachers();
    this.getLessons();
  }

  refresh(): void {
    this.lessonsSub = this.lessonsService
      .getLessons()
      .subscribe((lessons: Lessons[]) => {
        this.lessons = lessons;
        this.dataSource = new MatTableDataSource(this.lessons);
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
        this.changeDetectorRefs.detectChanges();
      });
  }

  getLessons(): void {
    this.spinner.show();
    this.lessonsSub = this.lessonsService.getLessons().subscribe(
      (lessons: Lessons[]) => {
        this.spinner.hide();
        this.lessons = lessons;
        this.dataSource = new MatTableDataSource(this.lessons);
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
      },
      (error) => {
        this.spinner.hide();
        this.alertify.error(error);
      }
    );
  }

  getTeachers(): void {
    this.teachersSub = this.userService.getTeachers().subscribe(
      (teachers: Teachers[]) => {
        this.teachers = teachers;
        this.dataSource = new MatTableDataSource(this.lessons);
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
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

  async deleteLesson(Id: number): Promise<void> {
    await this.alertify
      .openConfirmDialog(
        "Are you sure you want to delete this record?",
        "Delete"
      )
      .then((res) => {
        if (res.value) {
          this.spinner.show();
          this.lessonsSub = this.lessonsService.deleteLesson(Id).subscribe(
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

  upload_image = (files, id: string): void => {
    this.lessonid = id;
    let fileToUpload = <File>files[0];
    if (files.length === 0) return;
    const formData = new FormData();
    formData.append("file", fileToUpload, fileToUpload.name);
    this.filename = fileToUpload.name;
    var headers = new HttpHeaders();
    headers = headers.append(
      "Authorization",
      "Bearer " + localStorage.getItem("ecryptedToken")
    );
    this.http
      .post(
        this.baseUrl + "lessons/" + this.lessonid + "/editimage",
        formData,
        {
          reportProgress: true,
          observe: "events",
          headers: headers,
        }
      )
      .subscribe(
        (event) => {
          if (event.type === HttpEventType.UploadProgress) {
          } else if (event.type === HttpEventType.Response) {
            this.onUploadFinished.emit(event.body);
            this.refresh();
          }
        },
        (error) => {
          this.alertify.error(error);
        }
      );
  };

  createLesson(lessons: Lessons): void {
    const initialState = {
      lessons,
    };
    const dialogRef = this.dialog.open(LessonModalComponent, {
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

  async updateLesson(
    Lessonid: number,
    Teacherid: string,
    Lesson: string,
    Descr: string,
    Utubeurl: string
  ): Promise<void> {
    await this.alertify
      .openConfirmDialog(
        "Are you sure you want to update this record",
        "Update"
      )
      .then((res) => {
        if (res.value) {
          this.spinner.show();
          this.model.Lessonid = Lessonid;
          this.model.Teacherid = Teacherid;
          this.model.Lesson = Lesson;
          this.model.Descr = Descr;
          this.model.Utubeurl = Utubeurl;
          this.lessonsSub = this.lessonsService
            .updateLesson(Lessonid, this.model)
            .subscribe(
              (res) => {
                //this.lessons = lessons;
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
}
