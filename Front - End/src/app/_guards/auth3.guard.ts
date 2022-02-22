import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Router } from "@angular/router";
import { CanActivate } from "@angular/router/src/utils/preactivation";
import { AlertifyService } from "../_services/alertify.service";
import { AuthServiceSys } from "../_services/authSys.service";

@Injectable({
  providedIn: "root",
})
export class Auth3 implements CanActivate {
  path: ActivatedRouteSnapshot[];
  route: ActivatedRouteSnapshot;
  role: string;

  constructor(
    private authService: AuthServiceSys,
    private router: Router,
    private alertify: AlertifyService
  ) {}

  canActivate(): boolean {
    this.role = this.authService.getUserRole();

    //if logged in and is admin or teacher go
    if (
      this.authService.loggedIn() &&
      (this.authService.getUserRole() == "2" ||
        this.authService.getUserRole() == "1")
    ) {
      return true;
    }
    //if logged in and is user dont go
    if (this.authService.loggedIn() && this.role == "3") {
      this.alertify.error("You are not allowed to access this page!");
      this.router.navigate(["/lessons"]);
      return false;
    }
    //if not logged in dont go
    if (!this.authService.loggedIn()) {
      this.alertify.error("You are not allowed to access this page!");
      this.router.navigate(["/home"]);
      return false;
    }
  }
}
