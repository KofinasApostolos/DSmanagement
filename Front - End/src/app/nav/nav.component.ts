import {
  Component,
  HostListener,
  Input,
  OnDestroy,
  OnInit,
} from "@angular/core";
import { AuthServiceSys } from "src/app/_services/authSys.service";
import { AlertifyService } from "src/app/_services/alertify.service";
import { Router } from "@angular/router";
import { BehaviorSubject, Subject, Subscription } from "rxjs";
import { Communication } from "../_services/communication.service";

declare var FB: any;
@Component({
  selector: "app-nav",
  templateUrl: "./nav.component.html",
  styleUrls: ["./nav.component.css"],
})
export class NavComponent implements OnInit, OnDestroy {
  model: any = {};
  modelSocial: any = {};

  smallWidth: boolean = false;

  authSub: Subscription;

  @Input() photoUrl: string;
  @Input() token;

  constructor(
    public authService: AuthServiceSys,
    private alertify: AlertifyService,
    private router: Router,
    private communicationService: Communication
  ) {
    this.communicationService.imgUrl$.subscribe((img) => {
      this.photoUrl = img;
    });
    this.communicationService.decodedToken$.subscribe((token) => {
      this.token = token;
    });
  }

  ngOnDestroy(): void {}

  @HostListener("window:resize", ["$event"])
  getScreenSize(event?) {
    // this.scrHeight = window.innerHeight;
    // this.scrWidth = window.innerWidth;
    if (window.innerWidth < 768) this.smallWidth = true;
    else this.smallWidth = false;
    // console.log(this.scrHeight, this.scrWidth);
  }

  ngOnInit(): void {
    this.initFBLogin();

    if (
      localStorage.getItem("decodedToken") != null &&
      localStorage.getItem("user") != null
    ) {
      this.token = JSON.parse(localStorage.getItem("decodedToken"));
      this.photoUrl = JSON.parse(localStorage.getItem("user")).ImageUrl;
    }
  }

  private initFBLogin(): void {
    (window as any).fbAsyncInit = function () {
      FB.init({
        appId: "1237352470101481",
        cookie: true,
        xfbml: true,
        version: "v12.0",
      });
      FB.AppEvents.logPageView();
    };

    (function (d, s, id) {
      var js,
        fjs = d.getElementsByTagName(s)[0];
      if (d.getElementById(id)) {
        return;
      }
      js = d.createElement(s);
      js.id = id;
      js.src = "https://connect.facebook.net/en_US/sdk.js";
      fjs.parentNode.insertBefore(js, fjs);
    })(document, "script", "facebook-jssdk");
  }

  loginSocial() {
    window["FB"].login(
      (response) => {
        if (response.authResponse) {
          console.log("auth response from facebook --->>> ");
          console.log(response.authResponse);
          window["FB"].api(
            "/me",
            {
              fields: "last_name, first_name, email",
            },
            (userInfo) => {
              console.log("userInfo returned by facebook --->>> ");
              console.log(userInfo);
              this.loginSocialMethod(
                userInfo.last_name,
                userInfo.first_name,
                userInfo.email
              );
            }
          );
        } else {
          this.alertify.error("Login failed!");
        }
      },
      {
        scope: "email",
        return_scopes: true,
      }
    );
  }

  loginSocialMethod(lastname: string, firstname: string, email: string): void {
    this.modelSocial.lastname = lastname;
    this.modelSocial.firstname = firstname;
    this.modelSocial.email = email;
    this.authSub = this.authService.loginSocial(this.modelSocial).subscribe(
      () => {
        this.alertify.toastMessage("Logged in succesfully");
      },
      (error) => {
        this.alertify.error(error);
      },
      () => {
        this.router.navigate(["/lessons"]);
      }
    );
  }

  onSubmit(name: string): void {
    if (name === "Login") this.login();
    else this.router.navigate(["/home"]);
  }

  login(): void {
    this.authSub = this.authService.login(this.model).subscribe(
      () => {
        this.alertify.toastMessage("Logged in succesfully");
      },
      (error) => {
        this.alertify.error(error);
      },
      () => {
        this.router.navigate(["/lessons"]);
      }
    );
  }

  loggedIn(): boolean {
    const token = localStorage.getItem("ecryptedToken");
    return !!token; // if token has value return true, else false
  }

  logout(): void {
    this.alertify.toastMessage("logged out");
    localStorage.clear();
    this.authService.decodedToken = null;
    this.authService.currentUser = null;
  }
}
