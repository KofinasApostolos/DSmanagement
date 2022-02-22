import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Observable } from "rxjs";
import { User } from "../_models/user";
import { Teachers } from "../_models/teachers";

@Injectable({
  providedIn: "root",
})
export class UserService {
  private baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getUsersXLS(id: string, model: any): Observable<any[]> {
    var headers = new HttpHeaders();
    headers = headers.append(
      "Authorization",
      "Bearer " + localStorage.getItem("ecryptedToken")
    );
    headers = headers.append("Content-Type", "application/json");
    return this.http.post<any[]>(
      this.baseUrl + "users/" + id + "/usersxls",
      model,
      {
        headers: headers,
        // responseType: 'text'
      }
    );
  }

  getUsers(): Observable<User[]> {
    var headers = new HttpHeaders();
    headers = headers.append(
      "Authorization",
      "Bearer " + localStorage.getItem("ecryptedToken")
    );
    headers = headers.append("Content-Type", "application/json");
    return this.http.get<User[]>(this.baseUrl + "users", {
      headers: headers,
    });
  }

  getTeachers(): Observable<Teachers[]> {
    var headers = new HttpHeaders();
    headers = headers.append(
      "Authorization",
      "Bearer " + localStorage.getItem("ecryptedToken")
    );
    headers = headers.append("Content-Type", "application/json");
    return this.http.get<Teachers[]>(this.baseUrl + "users/teachers", {
      headers: headers,
    });
  }

  getUser(id: string): Observable<User> {
    var headers = new HttpHeaders();
    headers = headers.append(
      "Authorization",
      "Bearer " + localStorage.getItem("ecryptedToken")
    );
    headers = headers.append("Content-Type", "application/json");
    return this.http.get<User>(this.baseUrl + "users/" + id, {
      headers: headers,
    });
  }

  deleteUser(id: number): Observable<any> {
    var headers = new HttpHeaders();
    headers = headers.append(
      "Authorization",
      "Bearer " + localStorage.getItem("ecryptedToken")
    );
    headers = headers.append("Content-Type", "application/json");
    return this.http.delete(this.baseUrl + "users/" + id, {
      headers: headers,
    });
  }

  updateUser(model: any): Observable<any> {
    var headers = new HttpHeaders();
    headers = headers.append(
      "Authorization",
      "Bearer " + localStorage.getItem("ecryptedToken")
    );
    headers = headers.append("Content-Type", "application/json");
    return this.http.put(this.baseUrl + "users", model, {
      headers: headers,
    });
  }

  updateUserProfile(id: string, model: any): Observable<any> {
    var headers = new HttpHeaders();
    headers = headers.append(
      "Authorization",
      "Bearer " + localStorage.getItem("ecryptedToken")
    );
    headers = headers.append("Content-Type", "application/json");
    return this.http.put(this.baseUrl + "users/profile/" + id, model, {
      headers: headers,
    });
  }

  deletePhoto(id: number): Observable<any> {
    var headers = new HttpHeaders();
    headers = headers.append(
      "Authorization",
      "Bearer " + localStorage.getItem("ecryptedToken")
    );
    headers = headers.append("Content-Type", "application/json");
    return this.http.delete(this.baseUrl + "photo/" + id + "/photo/" + id, {
      headers: headers,
    });
  }

  changePassword(model: any): Observable<any> {
    var headers = new HttpHeaders();
    headers = headers.append(
      "Authorization",
      "Bearer " + localStorage.getItem("ecryptedToken")
    );
    headers = headers.append("Content-Type", "application/json");
    return this.http.post(this.baseUrl + "/users/changepassword", model, {
      headers: headers,
    });
  }

  register(model: any): Observable<any> {
    var headers = new HttpHeaders();
    headers = headers.append(
      "Authorization",
      "Bearer " + localStorage.getItem("ecryptedToken")
    );
    headers = headers.append("Content-Type", "application/json");
    return this.http.post(this.baseUrl + "/users/register", model, {
      headers: headers,
    });
  }

  resetPassword(model: any): Observable<any> {
    return this.http.post(this.baseUrl + "/users/resetpassword", model);
  }
}
