import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";
import { HttpClient } from "@angular/common/http";

@Injectable({
  providedIn: "root"
})
export class AdminService {
  private baseUrl = environment.apiUrl + "users";

  constructor(private http: HttpClient) {}

  getUsers() {
    return this.http.get(this.baseUrl);
  }
}
