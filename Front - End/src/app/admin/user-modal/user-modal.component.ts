import {
  Component,
  OnInit,
  EventEmitter,
  Output,
  OnDestroy,
} from "@angular/core";
import { User } from "src/app/_models/user";
import {
  FormGroup,
  FormControl,
  Validators,
  FormGroupDirective,
  NgForm,
} from "@angular/forms";
import { AlertifyService } from "src/app/_services/alertify.service";
import { Roles } from "src/app/_models/roles";
import { RolesService } from "src/app/_services/roles.service";
import { UserService } from "src/app/_services/user.service";
import { Subject, Subscription } from "rxjs";
import { environment } from "src/environments/environment";
import { HttpClient, HttpEventType, HttpHeaders } from "@angular/common/http";
import { Helpers } from "src/app/_helpers/helper";
import { ErrorStateMatcher } from "@angular/material";

export class MyErrorStateMatcher implements ErrorStateMatcher {
  isErrorState(
    control: FormControl | null,
    form: FormGroupDirective | NgForm | null
  ): boolean {
    const invalidCtrl = !!(control && control.invalid && control.parent.dirty);
    const invalidParent = !!(
      control &&
      control.parent &&
      control.parent.invalid &&
      control.parent.dirty
    );

    return invalidCtrl || invalidParent;
  }
}

@Component({
  selector: "app-user-modal",
  templateUrl: "./user-modal.component.html",
  styleUrls: ["./user-modal.component.css"],
})
export class UserModalComponent implements OnInit, OnDestroy {
  model: any = {};

  user: User;

  roles: Roles[];

  IsAdmin: string;
  publicid: string;
  filename: string;

  onClose: Subject<boolean>;

  showRemindPassword: boolean = false;
  collapse: boolean = true;

  private baseUrl = environment.apiUrl;

  private registerUserForm: FormGroup;

  usersSub: Subscription;
  rolesSub: Subscription;

  matcher = new MyErrorStateMatcher();

  @Output() photourl: string;
  @Output() public onUploadFinished = new EventEmitter();
  @Output() cancelRegister = new EventEmitter();

  constructor(
    private userService: UserService,
    private http: HttpClient,
    private alertify: AlertifyService,
    private rolesService: RolesService
  ) {}

  ngOnDestroy(): void {
    if (this.usersSub) this.usersSub.unsubscribe();
    if (this.rolesSub) this.rolesSub.unsubscribe();
  }

  getErrorMessage(controlType: string, controlName: string) {
    return Helpers.getErrorMessageReactiveForm(
      controlType,
      controlName,
      this.registerUserForm
    );
  }

  ngOnInit(): void {
    this.registerUserForm = new FormGroup(
      {
        username: new FormControl("", [
          Validators.required,
          Validators.minLength(4),
          Validators.maxLength(50),
        ]),
        filename: new FormControl(""),
        firstname: new FormControl("", [
          Validators.required,
          Validators.minLength(4),
          Validators.maxLength(50),
        ]),
        lastname: new FormControl("", [
          Validators.required,
          Validators.minLength(4),
          Validators.maxLength(50),
        ]),
        phonenumber: new FormControl("", [
          Validators.required,
          Validators.minLength(4),
          Validators.maxLength(50),
        ]),
        street: new FormControl("", [
          Validators.required,
          Validators.minLength(4),
          Validators.maxLength(50),
        ]),
        area: new FormControl("", [
          Validators.required,
          Validators.minLength(4),
          Validators.maxLength(50),
        ]),
        city: new FormControl("", [
          Validators.required,
          Validators.minLength(4),
          Validators.maxLength(50),
        ]),
        email: new FormControl("", [
          Validators.required,
          Validators.email,
          Validators.maxLength(50),
        ]),
        birthdate: new FormControl("", Validators.required),
        password: new FormControl("", [
          Validators.required,
          Validators.minLength(6),
          Validators.maxLength(50),
        ]),
        descr: new FormControl("", [
          Validators.required,
          Validators.minLength(50),
          Validators.maxLength(3000),
        ]),
        IsAdmin: new FormControl("", Validators.required),
        confirmPassword: new FormControl("", []),
      },
      { validators: this.passwordMatchValidator }
    );
    this.onClose = new Subject();
    this.getRoles();
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
          "users/" +
          parseInt(JSON.parse(localStorage.getItem("decodedToken")).nameid) +
          "/uploadimageadmin",
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

  getRoles(): void {
    this.rolesSub = this.rolesService.getRoles().subscribe(
      (roles: Roles[]) => {
        this.roles = roles;
      },
      (error) => {
        this.alertify.error(error);
      }
    );
  }

  changeRadioValue(): void {}

  passwordMatchValidator(g: FormGroup) {
    return g.get("password").value === g.get("confirmPassword").value
      ? null
      : { mismatch: true };
  }

  showPassword(input: any): void {
    input.type = input.type === "password" ? "text" : "password";
    if (input.type === "password") {
      this.collapse = false;
    } else this.collapse = true;
  }

  toggle(event): void {}

  public onConfirm(): void {
    this.model = this.registerUserForm.value;
    this.model.Imageurl = this.photourl;
    this.model.PublicId = this.publicid;
    this.usersSub = this.userService.register(this.model).subscribe(
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
}
