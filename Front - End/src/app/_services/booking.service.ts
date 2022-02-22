import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Observable } from "rxjs";
import { Userbookings } from "../_models/userbookings";

@Injectable({
  providedIn: "root",
})
export class BookingService {
  private baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  registerBooking(bookings: any): Observable<any> {
    var headers = new HttpHeaders();
    headers = headers.append(
      "Authorization",
      "Bearer " + localStorage.getItem("ecryptedToken")
    );
    headers = headers.append("Content-Type", "application/json");
    return this.http.post(this.baseUrl + "bookings/register", bookings, {
      headers: headers,
    });
  }

  getBookings(id: number): Observable<Userbookings[]> {
    var headers = new HttpHeaders();
    headers = headers.append(
      "Authorization",
      "Bearer " + localStorage.getItem("ecryptedToken")
    );
    headers = headers.append("Content-Type", "application/json");
    return this.http.get<Userbookings[]>(this.baseUrl + "bookings/" + id, {
      headers: headers,
    });
  }
}
