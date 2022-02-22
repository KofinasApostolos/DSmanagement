import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Observable, of } from "rxjs";
import { CDays } from "../_models/cdays";

@Injectable({
  providedIn: "root",
})
export class DaysService {
  private baseUrl = environment.apiUrl;
  days: any[] = [
    { Id: "1", Descr: "Monday" },
    { Id: "2", Descr: "Tuesday" },
    { Id: "3", Descr: "Wednesday" },
    { Id: "4", Descr: "Thursday" },
    { Id: "5", Descr: "Friday" },
    { Id: "6", Descr: "Saturday" },
    { Id: "7", Descr: "Sunday" },
  ];

  daysInt: any[] = [
    { Id: 1, Descr: "Monday" },
    { Id: 2, Descr: "Tuesday" },
    { Id: 3, Descr: "Wednesday" },
    { Id: 4, Descr: "Thursday" },
    { Id: 5, Descr: "Friday" },
    { Id: 6, Descr: "Saturday" },
    { Id: 7, Descr: "Sunday" },
  ];

  constructor(private http: HttpClient) {}

  getDays(): Observable<any[]> {
    return of(this.days);
  }

  getDaysInt(): Observable<any[]> {
    return of(this.daysInt);
  }

  getCustomDays(): Observable<CDays[]> {
    var headers = new HttpHeaders();
    headers = headers.append(
      "Authorization",
      "Bearer " + localStorage.getItem("ecryptedToken")
    );
    headers = headers.append("Content-Type", "application/json");
    return this.http.get<CDays[]>(this.baseUrl + "teachingprogram/cdays", {
      headers: headers,
    });
  }
}
