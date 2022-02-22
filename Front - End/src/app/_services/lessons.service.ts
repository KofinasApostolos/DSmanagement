import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Observable } from "rxjs";
import { Lessons } from "../_models/lessons";
import { CustomLesson } from "../_models/customlesson";
import { TeachingProgram } from "../_models/teachingprogram";
import { Subscriptions } from "../_models/subscriptions";
import { Teaching_Lessons } from "../_models/teaching_lessons";
import { Lesson } from "../admin/usersuggestion-management/usersuggestion-management.component";

@Injectable({
  providedIn: "root",
})
export class LessonsService {
  private baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getLessons(): Observable<Lessons[]> {
    var headers = new HttpHeaders();
    headers = headers.append(
      "Authorization",
      "Bearer " + localStorage.getItem("ecryptedToken")
    );
    headers = headers.append("Content-Type", "application/json");
    return this.http.get<Lessons[]>(this.baseUrl + "lessons", {
      headers: headers,
    });
  }

  getTeachingLessons(id: string): Observable<Teaching_Lessons[]> {
    var headers = new HttpHeaders();
    headers = headers.append(
      "Authorization",
      "Bearer " + localStorage.getItem("ecryptedToken")
    );
    headers = headers.append("Content-Type", "application/json");
    return this.http.get<Teaching_Lessons[]>(
      this.baseUrl + "lessons/" + id + "/teachinglessons",
      {
        headers: headers,
      }
    );
  }

  getCapacity(id: string): Observable<any[]> {
    var headers = new HttpHeaders();
    headers = headers.append(
      "Authorization",
      "Bearer " + localStorage.getItem("ecryptedToken")
    );
    headers = headers.append("Content-Type", "application/json");
    return this.http.get<any>(this.baseUrl + "bookings/capacity/" + id, {
      headers: headers,
      // responseType: "text",
    });
  }

  getLesson(id: string): Observable<Lessons> {
    var headers = new HttpHeaders();
    headers = headers.append(
      "Authorization",
      "Bearer " + localStorage.getItem("ecryptedToken")
    );
    headers = headers.append("Content-Type", "application/json");
    return this.http.get<Lessons>(this.baseUrl + "lessons/" + id, {
      headers: headers,
    });
  }

  getTeachingHours(id: string): Observable<TeachingProgram[]> {
    var headers = new HttpHeaders();
    headers = headers.append(
      "Authorization",
      "Bearer " + localStorage.getItem("ecryptedToken")
    );
    headers = headers.append("Content-Type", "application/json");
    return this.http.get<TeachingProgram[]>(
      this.baseUrl + "lessons/teachingprogram/" + id,
      {
        headers: headers,
      }
    );
  }

  getLessonCustom(id: string): Observable<CustomLesson> {
    var headers = new HttpHeaders();
    headers = headers.append(
      "Authorization",
      "Bearer " + localStorage.getItem("ecryptedToken")
    );
    headers = headers.append("Content-Type", "application/json");
    return this.http.get<CustomLesson>(this.baseUrl + "lessons/" + id, {
      headers: headers,
    });
  }

  getSubscriptions(id: string): Observable<Subscriptions[]> {
    var headers = new HttpHeaders();
    headers = headers.append(
      "Authorization",
      "Bearer " + localStorage.getItem("ecryptedToken")
    );
    headers = headers.append("Content-Type", "application/json");
    return this.http.get<Subscriptions[]>(
      this.baseUrl + "lessons/subscriptions/" + id,
      {
        headers: headers,
      }
    );
  }

  deleteLesson(id: number): Observable<any> {
    var headers = new HttpHeaders();
    headers = headers.append(
      "Authorization",
      "Bearer " + localStorage.getItem("ecryptedToken")
    );
    headers = headers.append("Content-Type", "application/json");
    return this.http.delete(this.baseUrl + "lessons/" + id, {
      headers: headers,
    });
  }

  updateLesson(id: number, model: any): Observable<any> {
    var headers = new HttpHeaders();
    headers = headers.append(
      "Authorization",
      "Bearer " + localStorage.getItem("ecryptedToken")
    );
    headers = headers.append("Content-Type", "application/json");
    return this.http.put(this.baseUrl + "lessons/" + id, model, {
      headers: headers,
    });
  }

  registerLesson(model: any): Observable<any> {
    var headers = new HttpHeaders();
    headers = headers.append(
      "Authorization",
      "Bearer " + localStorage.getItem("ecryptedToken")
    );
    headers = headers.append("Content-Type", "application/json");
    return this.http.post(this.baseUrl + "lessons/" + "register", model, {
      headers: headers,
    });
  }
}
