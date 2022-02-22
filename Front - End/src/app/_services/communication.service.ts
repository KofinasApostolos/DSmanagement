import { Injectable } from "@angular/core";
import { Observable, Subject, Subscriber } from "rxjs";

@Injectable({
  providedIn: "root",
})
export class Communication {
  constructor() {}

  private imageUrl = new Subject<string>();
  private decodedToken = new Subject<any>();
  private userToken = new Subject<any>();

  imgUrl$ = this.imageUrl.asObservable();
  decodedToken$ = this.decodedToken.asObservable();
  userToken$ = this.userToken.asObservable();

  emitImageUrl(data: string) {
    this.imageUrl.next(data);
  }

  emitDecodedToken(data: any) {
    this.decodedToken.next(data);
  }

  emitUserToken(data: any) {
    this.userToken.next(data);
  }
}
