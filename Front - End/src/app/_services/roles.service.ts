import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Roles } from "../_models/roles";
import { Observable } from "rxjs";

@Injectable({
  providedIn: "root",
})
export class RolesService {
  private baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getRoles(): Observable<Roles[]> {
    var headers = new HttpHeaders();
    headers = headers.append(
      "Authorization",
      "Bearer " + localStorage.getItem("ecryptedToken")
    );
    headers = headers.append("Content-Type", "application/json");
    return this.http.get<Roles[]>(this.baseUrl + "roles", {
      headers: headers,
    });
  }

  getRole(id: string): Observable<Roles> {
    var headers = new HttpHeaders();
    headers = headers.append(
      "Authorization",
      "Bearer " + localStorage.getItem("ecryptedToken")
    );
    headers = headers.append("Content-Type", "application/json");
    return this.http.get<Roles>(this.baseUrl + "roles/" + id, {
      headers: headers,
    });
  }

  deleteRole(id: number): Observable<any> {
    var headers = new HttpHeaders();
    headers = headers.append(
      "Authorization",
      "Bearer " + localStorage.getItem("ecryptedToken")
    );
    headers = headers.append("Content-Type", "application/json");
    return this.http.delete(this.baseUrl + "roles/" + id, {
      headers: headers,
    });
  }

  updateRole(model: any): Observable<any> {
    var headers = new HttpHeaders();
    headers = headers.append(
      "Authorization",
      "Bearer " + localStorage.getItem("ecryptedToken")
    );
    headers = headers.append("Content-Type", "application/json");
    return this.http.put(this.baseUrl + "roles", model, {
      headers: headers,
    });
  }

  registerRole(model: any): Observable<any> {
    var headers = new HttpHeaders();
    headers = headers.append(
      "Authorization",
      "Bearer " + localStorage.getItem("ecryptedToken")
    );
    headers = headers.append("Content-Type", "application/json");
    return this.http.post(this.baseUrl + "roles/" + "register", model, {
      headers: headers,
    });
  }
}
