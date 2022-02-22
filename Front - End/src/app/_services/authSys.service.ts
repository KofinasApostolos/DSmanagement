import { Injectable } from "@angular/core";
import { map } from "rxjs/operators";
import { JwtHelperService } from "@auth0/angular-jwt";
import { environment } from "src/environments/environment";
import { User } from "../_models/user";
import { BehaviorSubject, Observable } from "rxjs";
import { HttpClient } from "@angular/common/http";
import { Communication } from "./communication.service";

@Injectable({
  providedIn: "root",
})
export class AuthServiceSys {
  decodedToken: any = "";

  private baseUrl = environment.apiUrl + "auth/";

  private jwtHelper = new JwtHelperService();

  currentUser: User;
  user: User;
  model: User;

  constructor(
    private http: HttpClient,
    private communicationService: Communication
  ) {}

  login(model: any): Observable<any> {
    return this.http.post(this.baseUrl + "login", model).pipe(
      map((response: any) => {
        const user = response;
        if (user) {
          localStorage.setItem("user", JSON.stringify(user.user));
          this.decodedToken = this.jwtHelper.decodeToken(user.token);
          localStorage.setItem(
            "decodedToken",
            JSON.stringify(this.decodedToken)
          );
          localStorage.setItem("ecryptedToken", user.token);

          console.log(JSON.parse(localStorage.getItem("decodedToken")));
          console.log(JSON.parse(localStorage.getItem("user")));
          console.log(localStorage.getItem("ecryptedToken"));

          var userObj = JSON.parse(localStorage.getItem("user"));

          if (userObj.ImageUrl === null) {
            userObj.ImageUrl = "../../assets/default.png";
            localStorage.setItem("user", JSON.stringify(userObj));
          }

          this.communicationService.emitImageUrl(
            JSON.parse(localStorage.getItem("user")).ImageUrl
          );

          this.communicationService.emitDecodedToken(
            JSON.parse(localStorage.getItem("decodedToken"))
          );
        }
      })
    );
  }

  loginSocial(model: any): Observable<any> {
    console.log("loginSocial model --->>>");
    console.log(model);
    return this.http.post(this.baseUrl + "loginsocial", model).pipe(
      map((response: any) => {
        console.log("loginSocial response --->>>");
        console.log(response);
        const user = response;
        if (user) {
          localStorage.setItem("user", JSON.stringify(user.user));
          this.decodedToken = this.jwtHelper.decodeToken(user.token);
          localStorage.setItem(
            "decodedToken",
            JSON.stringify(this.decodedToken)
          );
          localStorage.setItem("ecryptedToken", user.token);
          console.log(JSON.parse(localStorage.getItem("decodedToken")));
          console.log(JSON.parse(localStorage.getItem("user")));
          console.log(localStorage.getItem("ecryptedToken"));

          var userObj = JSON.parse(localStorage.getItem("user"));

          if (userObj.ImageUrl === null) {
            userObj.ImageUrl = "../../assets/default.png";
            localStorage.setItem("user", JSON.stringify(userObj));
          }

          this.communicationService.emitImageUrl(
            JSON.parse(localStorage.getItem("user")).ImageUrl
          );

          this.communicationService.emitDecodedToken(
            JSON.parse(localStorage.getItem("decodedToken"))
          );

        }
      })
    );
  }

  getUserName() {
    // tslint:disable-next-line:no-unused-expression
    return JSON.parse(localStorage.getItem("decodedToken")).unique_name;
  }

  loggedIn() {
    const token = localStorage.getItem("ecryptedToken");
    return !this.jwtHelper.isTokenExpired(token);
  }

  getUserRole() {
    if (JSON.parse(localStorage.getItem("decodedToken")).role === "1")
      return "1";
    else if (JSON.parse(localStorage.getItem("decodedToken")).role === "2")
      return "2";
  }
}
