import {
  Component,
  OnInit,
  Output,
  EventEmitter,
  OnDestroy,
  Input,
} from "@angular/core";
import {
  FormControl,
  FormGroup,
  FormGroupDirective,
  NgForm,
  Validators,
} from "@angular/forms";
import { FileUploader } from "ng2-file-upload";
import { environment } from "src/environments/environment";
import { Photo } from "src/app/_models/photo";
import { UserService } from "src/app/_services/user.service";
import { AlertifyService } from "src/app/_services/alertify.service";
import { NgxSpinnerService } from "ngx-spinner";
import { User } from "../_models/user";
import { Subscription } from "rxjs";
import { Communication } from "../_services/communication.service";
import { Helpers } from "../_helpers/helper";
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
  selector: "app-userprofile",
  templateUrl: "./userprofile.component.html",
  styleUrls: ["./userprofile.component.css"],
})
export class UserprofileComponent implements OnInit, OnDestroy {
  private model: any = {};
  private uploader: FileUploader;

  private userProfileForm: FormGroup;

  private hasBaseDropZoneOver: boolean = false;
  private collapse: boolean = true;

  private baseUrl = environment.apiUrl;

  usersSub: Subscription;

  photoUrl: string;

  matcher = new MyErrorStateMatcher();

  @Output() cancelUserProfile = new EventEmitter();

  constructor(
    private userService: UserService,
    private alertifyService: AlertifyService,
    private spinner: NgxSpinnerService,
    private communicationService: Communication
  ) {}

  ngOnDestroy(): void {
    this.userProfileForm.reset();
    if (this.usersSub) this.usersSub.unsubscribe();
  }

  ngOnInit(): void {
    this.spinner.show();
    this.initializeUploader();
    this.initForm();
    this.initControls();
    this.spinner.hide();
  }

  initControls(): void {
    this.usersSub = this.userService
      .getUser(JSON.parse(localStorage.getItem("decodedToken")).nameid)
      .subscribe(
        (user: User) => {
          this.userProfileForm.controls["username"].setValue(user.Username);
          this.userProfileForm.controls["descr"].setValue(user.Descr);
          this.userProfileForm.controls["firstname"].setValue(user.FirstName);
          this.userProfileForm.controls["lastname"].setValue(user.LastName);
          this.userProfileForm.controls["email"].setValue(user.Email);
          this.userProfileForm.controls["street"].setValue(user.Street);
          this.userProfileForm.controls["area"].setValue(user.Area);
          this.userProfileForm.controls["city"].setValue(user.City);
          this.userProfileForm.controls["birthdate"].setValue(
            Helpers.dateConvert(user.Birthdate.toString())
          );
          this.userProfileForm.controls["phonenumber"].setValue(
            user.Phonenumber
          );
          this.userProfileForm.controls["password"].setValue(user.Password);
          this.userProfileForm.controls["confirmPassword"].setValue(
            user.Password
          );
          this.photoUrl = JSON.parse(localStorage.getItem("user")).ImageUrl;
        },
        (error) => {
          this.alertifyService.error(error);
        }
      );
  }

  initForm(): void {
    this.userProfileForm = new FormGroup(
      {
        username: new FormControl({ disabled: true }),
        firstname: new FormControl("", [
          Validators.required,
          Validators.maxLength(50),
          Validators.minLength(4),
        ]),
        birthdate: new FormControl("", [Validators.required]),
        lastname: new FormControl("", [
          Validators.required,
          Validators.maxLength(50),
          Validators.minLength(4),
        ]),
        descr: new FormControl("", [
          Validators.required,
          Validators.maxLength(3000),
          Validators.minLength(50),
        ]),
        street: new FormControl("", [
          Validators.required,
          Validators.maxLength(50),
          Validators.minLength(4),
        ]),
        phonenumber: new FormControl("", [
          Validators.required,
          Validators.maxLength(50),
          Validators.minLength(4),
        ]),
        city: new FormControl("", [
          Validators.required,
          Validators.maxLength(50),
          Validators.minLength(4),
        ]),
        area: new FormControl("", [
          Validators.required,
          Validators.maxLength(50),
          Validators.minLength(4),
        ]),
        email: new FormControl("", [
          Validators.required,
          Validators.minLength(4),
          Validators.maxLength(50),
        ]),
        password: new FormControl("", [
          Validators.required,
          Validators.minLength(6),
          Validators.maxLength(50),
        ]),
        filename: new FormControl(""),
        confirmPassword: new FormControl("", []),
      },
      { validators: this.passwordMatchValidator }
    );
  }

  getErrorMessage(controlType: string, controlName: string) {
    return Helpers.getErrorMessageReactiveForm(
      controlType,
      controlName,
      this.userProfileForm
    );
  }

  updateProfile(): void {
    this.spinner.show();
    this.model = this.userProfileForm.value;
    this.usersSub = this.userService
      .updateUserProfile(
        JSON.parse(localStorage.getItem("decodedToken")).nameid,
        this.model
      )
      .subscribe(
        () => {
          var user = JSON.parse(localStorage.getItem("user"));
          user.Area = this.userProfileForm.controls["area"].value;
          user.City = this.userProfileForm.controls["city"].value;
          user.DateofBirth = this.userProfileForm.controls["birthdate"].value;
          user.Descr = this.userProfileForm.controls["descr"].value;
          user.Email = this.userProfileForm.controls["email"].value;
          user.FirstName = this.userProfileForm.controls["firstname"].value;
          user.LastName = this.userProfileForm.controls["lastname"].value;
          user.Password = this.userProfileForm.controls["password"].value;
          user.Phonenumber = this.userProfileForm.controls["phonenumber"].value;
          user.Street = this.userProfileForm.controls["street"].value;
          localStorage.setItem("user", JSON.stringify(user));
          this.alertifyService.success("Update has been succesful");
          this.spinner.hide();
        },
        (error) => {
          this.spinner.hide();
          this.alertifyService.error(error);
        }
      );
  }

  passwordMatchValidator(g: FormGroup): any {
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

  fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }

  initializeUploader(): void {
    const userid = parseInt(
      JSON.parse(localStorage.getItem("decodedToken")).nameid
    );
    this.uploader = new FileUploader({
      url: this.baseUrl + "users/" + userid + "/uploadimage",
      isHTML5: true,
      allowedFileType: ["image"],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024,
      authToken: "Bearer " + localStorage.getItem("ecryptedToken"),
    });

    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false;
    };

    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response) {
        const res: Photo = JSON.parse(response);
        var user = JSON.parse(localStorage.getItem("user"));
        user.ImageUrl = res.Url;
        localStorage.setItem("user", JSON.stringify(user));
        this.photoUrl = JSON.parse(localStorage.getItem("user")).ImageUrl;
        this.photoUrl = res.Url;
        this.communicationService.emitImageUrl(this.photoUrl);
      }
    };
  }
}
