import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Observable, of } from "rxjs";
import { Subscriptions } from "../_models/subscriptions";
import { SubscriptionState } from "../_models/subscriptionstate";

@Injectable({
  providedIn: "root",
})
export class SubscriptionsService {
  private baseUrl = environment.apiUrl;
  states: SubscriptionState[] = [
    { Id: 1, Descr: "Activated" },
    { Id: 2, Descr: "Deactivated" },
  ];
  constructor(private http: HttpClient) {}

  getSubscriptions(): Observable<Subscriptions[]> {
    var headers = new HttpHeaders();
    headers = headers.append(
      "Authorization",
      "Bearer " + localStorage.getItem("ecryptedToken")
    );
    headers = headers.append("Content-Type", "application/json");
    return this.http.get<Subscriptions[]>(this.baseUrl + "subscriptions", {
      headers: headers,
    });
  }

  getSubscriptionStates(): Observable<SubscriptionState[]> {
    return of(this.states);
  }

  updateSubscription(model: any): Observable<any> {
    var headers = new HttpHeaders();
    headers = headers.append(
      "Authorization",
      "Bearer " + localStorage.getItem("ecryptedToken")
    );
    headers = headers.append("Content-Type", "application/json");
    return this.http.put(this.baseUrl + "subscriptions", model, {
      headers: headers,
    });
  }

  deleteSubscription(id: number): Observable<any> {
    var headers = new HttpHeaders();
    headers = headers.append(
      "Authorization",
      "Bearer " + localStorage.getItem("ecryptedToken")
    );
    headers = headers.append("Content-Type", "application/json");
    return this.http.delete(this.baseUrl + "subscriptions/" + id, {
      headers: headers,
    });
  }

  registerSubscription(model: any): Observable<any> {
    var headers = new HttpHeaders();
    headers = headers.append(
      "Authorization",
      "Bearer " + localStorage.getItem("ecryptedToken")
    );
    headers = headers.append("Content-Type", "application/json");
    return this.http.post(this.baseUrl + "subscriptions/" + "register", model, {
      headers: headers,
    });
  }
}
