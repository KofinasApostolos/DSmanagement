import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Router } from "@angular/router";
import { CanActivate } from "@angular/router/src/utils/preactivation";
import { AlertifyService } from "../_services/alertify.service";
import { AuthServiceSys } from "../_services/authSys.service";

@Injectable({
  providedIn: "root",
})
export class Auth5 implements CanActivate {
  path: ActivatedRouteSnapshot[];
  route: ActivatedRouteSnapshot;
  role: string;
  constructor(private authService: AuthServiceSys, private router: Router) {}

  canActivate(): boolean {
    //if is logged in not go to signup page
    if (this.authService.loggedIn()) {
      this.router.navigate(["/lessons"]);
      return false;
    } else {
      return true;
    }
  }
}
