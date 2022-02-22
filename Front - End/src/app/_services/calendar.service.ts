import { Injectable } from '@angular/core';
import { environment } from "src/environments/environment";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Observable } from "rxjs";
import { CalendarEvent } from "calendar-utils";
import { TempRegisters } from '../_models/tempregisters';

@Injectable({
  providedIn: 'root'
})
export class CalendarService {
  private baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getBookings(): Observable<CalendarEvent[]> {
    var headers = new HttpHeaders();
    headers = headers.append(
      "Authorization",
      "Bearer " + localStorage.getItem("ecryptedToken")
    );
    headers = headers.append("Content-Type", "application/json");
    return this.http.get<CalendarEvent[]>(this.baseUrl + "bookings/calendar", {
      headers: headers
    });
  }

  getBookingsByDate(dt: string): Observable<TempRegisters[]> {
    var headers = new HttpHeaders();
    headers = headers.append(
      "Authorization",
      "Bearer " + localStorage.getItem("ecryptedToken")
    );
    headers = headers.append("Content-Type", "application/json");
    return this.http.get<TempRegisters[]>(this.baseUrl + "bookings/detailed/" + dt, {
      headers: headers
    });
  }

}
