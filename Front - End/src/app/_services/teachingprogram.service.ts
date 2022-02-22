import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Observable } from "rxjs";
import { TeachingProgram } from '../_models/teachingprogram';
import { TeachingProgramTemp } from '../_models/teachingprogramtemp';

@Injectable({
  providedIn: 'root'
})
export class TeachingprogramService {

  private baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getTeachingProgramsTempUser(id: string): Observable<TeachingProgramTemp[]> {
    var headers = new HttpHeaders();
    headers = headers.append(
      "Authorization",
      "Bearer " + localStorage.getItem("ecryptedToken")
    );
    headers = headers.append("Content-Type", "application/json");
    return this.http.get<TeachingProgramTemp[]>(this.baseUrl + "teachingprogram/" + id + "/getsuggestionsuser", {
      headers: headers
    });
  }

  getTeachingProgramXLS(id: string, model: any): Observable<any[]> {
    var headers = new HttpHeaders();
    headers = headers.append(
      "Authorization",
      "Bearer " + localStorage.getItem("ecryptedToken")
    );
    headers = headers.append("Content-Type", "application/json");
    return this.http.post<any[]>(this.baseUrl + "teachingprogram/" + id + "/tpxls", model,
      {
        headers: headers,
       // responseType: 'text'
      });
  }

  transferToCoreProgram(model: any): Observable<any> {
    var headers = new HttpHeaders();
    headers = headers.append(
      "Authorization",
      "Bearer " + localStorage.getItem("ecryptedToken")
    );
    headers = headers.append("Content-Type", "application/json");
    return this.http.post(this.baseUrl + "teachingprogram/transfer2core", model, {
      headers: headers
    });
  }

  deleteSuggestionUser(model: any, id: string): Observable<any> {
    var headers = new HttpHeaders();
    headers = headers.append(
      "Authorization",
      "Bearer " + localStorage.getItem("ecryptedToken")
    );
    headers = headers.append("Content-Type", "application/json");
    return this.http.post(this.baseUrl + "teachingprogram/delsuggestionuser/" + id, model, {
      headers: headers
    });
  }

  deleteSuggestionAdmin(id: number): Observable<any> {
    var headers = new HttpHeaders();
    headers = headers.append(
      "Authorization",
      "Bearer " + localStorage.getItem("ecryptedToken")
    );
    headers = headers.append("Content-Type", "application/json");
    return this.http.delete(this.baseUrl + "teachingprogram/delsuggestionadmin" + id, {
      headers: headers
    });
  }

  getTeachingPrograms(): Observable<TeachingProgram[]> {
    var headers = new HttpHeaders();
    headers = headers.append(
      "Authorization",
      "Bearer " + localStorage.getItem("ecryptedToken")
    );
    headers = headers.append("Content-Type", "application/json");
    return this.http.get<TeachingProgram[]>(this.baseUrl + "teachingprogram", {
      headers: headers
    });
  }

  getTeachingProgram(id: string): Observable<TeachingProgram> {
    var headers = new HttpHeaders();
    headers = headers.append(
      "Authorization",
      "Bearer " + localStorage.getItem("ecryptedToken")
    );
    headers = headers.append("Content-Type", "application/json");
    return this.http.get<TeachingProgram>(this.baseUrl + "teachingprogram/" + id, {
      headers: headers
    });
  }

  deleteTeachingProgram(id: number): Observable<any> {
    var headers = new HttpHeaders();
    headers = headers.append(
      "Authorization",
      "Bearer " + localStorage.getItem("ecryptedToken")
    );
    headers = headers.append("Content-Type", "application/json");
    return this.http.delete(this.baseUrl + "teachingprogram/" + id, {
      headers: headers
    });
  }

  updateTeachingProgram(model: any): Observable<any> {
    var headers = new HttpHeaders();
    headers = headers.append(
      "Authorization",
      "Bearer " + localStorage.getItem("ecryptedToken")
    );
    headers = headers.append("Content-Type", "application/json");
    return this.http.put(this.baseUrl + "teachingprogram", model, {
      headers: headers
    });
  }

  updateSuggestionUser(id: string, model: any): Observable<any> {
    var headers = new HttpHeaders();
    headers = headers.append(
      "Authorization",
      "Bearer " + localStorage.getItem("ecryptedToken")
    );
    headers = headers.append("Content-Type", "application/json");
    return this.http.put(this.baseUrl + "teachingprogram/" + id + "/updatesuggestion", model, {
      headers: headers
    });
  }

  registerTeachingProgram(model: any): Observable<any> {
    var headers = new HttpHeaders();
    headers = headers.append(
      "Authorization",
      "Bearer " + localStorage.getItem("ecryptedToken")
    );
    headers = headers.append("Content-Type", "application/json");
    return this.http.post(this.baseUrl + "teachingprogram/register", model, {
      headers: headers
    });
  }

  submitSuggestion(model: any): Observable<any> {
    var headers = new HttpHeaders();
    headers = headers.append(
      "Authorization",
      "Bearer " + localStorage.getItem("ecryptedToken")
    );
    headers = headers.append("Content-Type", "application/json");
    return this.http.post(this.baseUrl + "teachingprogram/usersuggestion", model, {
      headers: headers
    });
  }

  registerClassroom(model: any): Observable<any> {
    var headers = new HttpHeaders();
    headers = headers.append(
      "Authorization",
      "Bearer " + localStorage.getItem("ecryptedToken")
    );
    headers = headers.append("Content-Type", "application/json");
    return this.http.post(this.baseUrl + "teachingprogram/registerclass", model, {
      headers: headers
    });
  }
}
