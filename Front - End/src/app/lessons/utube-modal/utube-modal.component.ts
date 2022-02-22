import { Component, Input, OnInit, ViewChild } from "@angular/core";
import { DomSanitizer, SafeResourceUrl } from "@angular/platform-browser";
import { Subject } from "rxjs";

@Component({
  selector: "app-utube-modal",
  templateUrl: "./utube-modal.component.html",
  styleUrls: ["./utube-modal.component.css"],
})
export class UtubeModalComponent implements OnInit {
  public onClose: Subject<boolean>;

  @ViewChild("player") player: any;
  videoId: SafeResourceUrl;

  @Input()
  set id(id: SafeResourceUrl) {
    this.videoId = id;
  }

  constructor(public sanitizer: DomSanitizer) {}

  ngOnInit(): void {
    this.onClose = new Subject();
    this.id = this.sanitizer.bypassSecurityTrustResourceUrl(
      `https://www.youtube.com/embed/` +
        `${JSON.parse(localStorage.getItem("lesson")).Utubeurl}`
    );
  }

  public onCancel(): void {
    this.onClose.next(false);
  }
}
