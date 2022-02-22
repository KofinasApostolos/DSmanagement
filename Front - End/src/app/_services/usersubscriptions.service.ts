import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Observable } from "rxjs";
import { UserSubscription } from "../_models/usersubscription";

@Injectable({
  providedIn: "root",
})
export class UsersubscriptionsService {
  private baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getUserSubsXLS(id: string, model: any): Observable<any[]> {
    var headers = new HttpHeaders();
    headers = headers.append(
      "Authorization",
      "Bearer " + localStorage.getItem("ecryptedToken")
    );
    headers = headers.append("Content-Type", "application/json");
    return this.http.post<any[]>(
      this.baseUrl + "usersubscriptions/" + id + "/userssubsxls",
      model,
      {
        headers: headers,
        //responseType: "text",
      }
    );
  }

  getUsersubscriptions(): Observable<UserSubscription[]> {
    var headers = new HttpHeaders();
    headers = headers.append(
      "Authorization",
      "Bearer " + localStorage.getItem("ecryptedToken")
    );
    headers = headers.append("Content-Type", "application/json");
    return this.http.get<UserSubscription[]>(
      this.baseUrl + "usersubscriptions",
      {
        headers: headers,
      }
    );
  }

  getUsersubscription(id: string): Observable<UserSubscription> {
    var headers = new HttpHeaders();
    headers = headers.append(
      "Authorization",
      "Bearer " + localStorage.getItem("ecryptedToken")
    );
    headers = headers.append("Content-Type", "application/json");
    return this.http.get<UserSubscription>(
      this.baseUrl + "usersubscriptions/" + id,
      {
        headers: headers,
      }
    );
  }

  deleteUsersubscription(id: number): Observable<any> {
    var headers = new HttpHeaders();
    headers = headers.append(
      "Authorization",
      "Bearer " + localStorage.getItem("ecryptedToken")
    );
    headers = headers.append("Content-Type", "application/json");
    return this.http.delete(this.baseUrl + "usersubscriptions/" + id, {
      headers: headers,
    });
  }

  updateUsersubscription(model: any): Observable<any> {
    var headers = new HttpHeaders();
    headers = headers.append(
      "Authorization",
      "Bearer " + localStorage.getItem("ecryptedToken")
    );
    headers = headers.append("Content-Type", "application/json");
    return this.http.put(this.baseUrl + "usersubscriptions", model, {
      headers: headers,
    });
  }

  registerUsersubscription(model: any): Observable<any> {
    var headers = new HttpHeaders();
    headers = headers.append(
      "Authorization",
      "Bearer " + localStorage.getItem("ecryptedToken")
    );
    headers = headers.append("Content-Type", "application/json");
    return this.http.post(this.baseUrl + "usersubscriptions/register", model, {
      headers: headers,
    });
  }
}
