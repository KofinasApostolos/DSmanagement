import { Injectable } from "@angular/core";
import {
  ActivatedRouteSnapshot,
  Router
} from "@angular/router";
import { CanActivate } from "@angular/router/src/utils/preactivation";
import { AlertifyService } from "../_services/alertify.service";
import { AuthServiceSys } from "../_services/authSys.service";

@Injectable({
  providedIn: "root"
})
export class AuthGuard implements CanActivate {
  path: ActivatedRouteSnapshot[];
  route: ActivatedRouteSnapshot;

  constructor(
    private authService: AuthServiceSys,
    private router: Router,
    private alertify: AlertifyService
  ) { }

  canActivate(): boolean {
    //if logged in go
    if (this.authService.loggedIn()) {
      return true;
    }
    //if not logged in dont go    
    this.alertify.error("You are not allowed to access this page!");
    this.router.navigate(["/home"]);
    return false;
  }
}
