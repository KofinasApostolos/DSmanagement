import {
  Component,
  OnInit,
  Output,
  EventEmitter,
  OnDestroy,
} from "@angular/core";
import { AlertifyService } from "../_services/alertify.service";
import {
  FormControl,
  FormGroup,
  FormGroupDirective,
  NgForm,
  Validators,
} from "@angular/forms";
import { UserService } from "../_services/user.service";
import { NgxSpinnerService } from "ngx-spinner";
import { Subscription } from "rxjs";
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
  selector: "app-home",
  templateUrl: "./home.component.html",
  styleUrls: ["./home.component.css"],
})
export class HomeComponent implements OnInit, OnDestroy {
  model: any = {};

  isActive: boolean = false;
  showRemindPassword: boolean = false;
  private collapse: boolean = true;

  registerForm: FormGroup;
  resetpassForm: FormGroup;

  usersSub: Subscription;

  matcher = new MyErrorStateMatcher();

  @Output() cancelRegister = new EventEmitter();
  @Output() cancelResetPass = new EventEmitter();

  constructor(
    private userService: UserService,
    private alertify: AlertifyService,
    private spinner: NgxSpinnerService
  ) {}

  ngOnDestroy(): void {
    if (this.usersSub) this.usersSub.unsubscribe();
  }

  ngOnInit(): void {
    this.spinner.show();
    this.showRemindPassword = false;
    this.resetpassForm = new FormGroup({
      email: new FormControl("", [
        Validators.required,
        Validators.email,
        Validators.minLength(4),
        Validators.maxLength(50),
      ]),
    });
    this.registerForm = new FormGroup(
      {
        birthdate: new FormControl("", Validators.required),
        email: new FormControl("", [
          Validators.required,
          Validators.email,
          Validators.minLength(4),
          Validators.maxLength(50),
        ]),
        username: new FormControl("", [
          Validators.required,
          Validators.minLength(4),
          Validators.maxLength(50),
        ]),
        password: new FormControl("", [
          Validators.required,
          Validators.minLength(4),
          Validators.maxLength(50),
        ]),
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
        city: new FormControl("", [
          Validators.required,
          Validators.minLength(4),
          Validators.maxLength(50),
        ]),
        area: new FormControl("", [
          Validators.required,
          Validators.minLength(4),
          Validators.maxLength(50),
        ]),
        confirmPassword: new FormControl("", []),
      },
      { validators: this.passwordMatchValidator }
    );
    this.spinner.hide();
  }

  getErrorMessage(controlType: string, controlName: string) {
    return Helpers.getErrorMessageReactiveForm(
      controlType,
      controlName,
      this.registerForm
    );
  }

  passwordMatchValidator(g: FormGroup) {
    return g.get("password").value === g.get("confirmPassword").value
      ? null
      : { mismatch: true };
  }

  register(): void {
    this.spinner.show();
    this.model = this.registerForm.value;
    this.usersSub = this.userService.register(this.model).subscribe(
      (res) => {
        this.registerForm.reset();
        this.alertify.success(res.Message);
        this.spinner.hide();
      },
      (error) => {
        this.alertify.error(error);
        this.spinner.hide();
      }
    );
  }

  showPassword(input: any): void {
    input.type = input.type === "password" ? "text" : "password";
    if (input.type === "password") {
      this.collapse = false;
    } else this.collapse = true;
  }

  cancel(): void {
    this.cancelRegister.emit(false);
  }

  remindPassword(): void {
    this.showRemindPassword = this.showRemindPassword ? false : true;
  }

  getLostPassword(): void {
    this.spinner.show();
    this.model = this.resetpassForm.value;
    this.usersSub = this.userService.resetPassword(this.model).subscribe(
      (res) => {
        this.alertify.success(res.Message);
        this.spinner.hide();
      },
      (error) => {
        this.alertify.error(error);
        this.spinner.hide();
      }
    );
  }
}
