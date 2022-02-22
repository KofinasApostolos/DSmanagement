import { Injectable } from "@angular/core";
import {
  ActivatedRouteSnapshot,
  Router
} from "@angular/router";
import { CanActivate } from "@angular/router/src/utils/preactivation";
import { AlertifyService } from "../_services/alertify.service";
import { AuthServiceSys } from "../_services/authSys.service";

@Injectable({
  providedIn: "root",
})
export class Auth4 implements CanActivate {
  path: ActivatedRouteSnapshot[];
  route: ActivatedRouteSnapshot;
  role: string;
  constructor(
    private authService: AuthServiceSys,
    private router: Router,
    private alertify: AlertifyService
  ) {}

  canActivate(): boolean {
    //if is logged in and not from social
    this.role = this.authService.getUserRole();
    if (this.authService.loggedIn() && this.role != "3") {
      return true;
    }
    //if is logged in and from social dont go
    if (this.authService.loggedIn() && this.role == "3") {
      this.alertify.error("You are not allowed to access this page!");
      this.router.navigate(["/lessons"]);
      return false;
    }
    //if is not logged in dont go
    if (!this.authService.loggedIn()) {
      this.alertify.error("You are not allowed to access this page!");
      this.router.navigate(["/home"]);
      return false;
    }
  }
}
