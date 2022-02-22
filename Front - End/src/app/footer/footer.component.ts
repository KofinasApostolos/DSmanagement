import { Component, OnInit } from "@angular/core";

// just an interface for type safety.
export interface marker {
  lat: number;
  lng: number;
  label?: string;
  draggable: boolean;
}

@Component({
  selector: "app-footer",
  templateUrl: "./footer.component.html",
  styleUrls: ["./footer.component.css"],
})
export class FooterComponent {
  lat: number = 37.98381;
  lng: number = 23.727539;

  constructor() {}

}
