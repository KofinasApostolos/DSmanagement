import { Injectable } from '@angular/core';
import { environment } from "src/environments/environment";
import { HttpClient, HttpHeaders } from "@angular/common/http";

@Injectable({
  providedIn: 'root'
})
export class ClosedayService {
  private baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  closeDay() {
    var headers = new HttpHeaders();
    headers = headers.append(
      "Authorization",
      "Bearer " + localStorage.getItem("ecryptedToken")
    );
    headers = headers.append("Content-Type", "application/json");
    return this.http.post(this.baseUrl + "closeday", {
      headers: headers
    });
  }

}
