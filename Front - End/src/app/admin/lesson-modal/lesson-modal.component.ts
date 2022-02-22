import {
  Component,
  EventEmitter,
  OnDestroy,
  OnInit,
  Output,
} from "@angular/core";
import { AlertifyService } from "src/app/_services/alertify.service";
import { Teachers } from "src/app/_models/teachers";
import { LessonsService } from "src/app/_services/lessons.service";
import { Subject, Subscription } from "rxjs";
import { User } from "src/app/_models/user";
import { Roles } from "src/app/_models/roles";
import { environment } from "src/environments/environment";
import { UserService } from "src/app/_services/user.service";
import {
  HttpClient,
  HttpEventType,
  HttpHeaders,
  HttpParams,
} from "@angular/common/http";
import { Helpers } from "src/app/_helpers/helper";
import { FormControl } from "@angular/forms";

@Component({
  selector: "app-lesson-modal",
  templateUrl: "./lesson-modal.component.html",
  styleUrls: ["./lesson-modal.component.css"],
})
export class LessonModalComponent implements OnInit, OnDestroy {
  model: any = {};

  teachers: Teachers[];
  roles: Roles[];
  user: User;

  public onClose: Subject<boolean>;

  private baseUrl = environment.apiUrl;

  showRemindPassword: boolean = false;
  collapse: boolean = true;

  IsAdmin: string;
  filename: string;
  publicid: string;

  teachersSub: Subscription;
  lessonsSub: Subscription;

  @Output() photourl: string;
  @Output() public onUploadFinished = new EventEmitter();

  constructor(
    private userService: UserService,
    private lessonsService: LessonsService,
    private http: HttpClient,
    private alertify: AlertifyService
  ) {}

  ngOnDestroy(): void {
    if (this.teachersSub) this.teachersSub.unsubscribe();
    if (this.lessonsSub) this.lessonsSub.unsubscribe();
  }

  ngOnInit(): void {
    this.getTeachers();
    this.onClose = new Subject();
  }

  uploadImage(files): void {
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
        this.baseUrl +
          "lessons/" +
          parseInt(JSON.parse(localStorage.getItem("decodedToken")).nameid) +
          "/uploadimage",
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
            const options: {
              params?: any;
            } = {
              params: event.body,
            };
            this.photourl = options.params.Url;
            this.publicid = options.params.PublicId;
          }
        },
        (error) => {
          this.alertify.error(error);
        }
      );
  }

  public onConfirm(): void {
    this.model.Imageurl = this.photourl;
    this.model.PublicId = this.publicid;
    this.lessonsSub = this.lessonsService.registerLesson(this.model).subscribe(
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

  getTeachers(): void {
    this.teachersSub = this.userService.getTeachers().subscribe(
      (teachers: Teachers[]) => {
        this.teachers = teachers;
      },
      (error) => {
        this.alertify.error(error);
      }
    );
  }
}
