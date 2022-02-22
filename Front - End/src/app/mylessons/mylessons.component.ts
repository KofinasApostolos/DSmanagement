import { Teaching_Lessons } from "../_models/teaching_lessons";
import { Component, OnDestroy, OnInit, ViewChild } from "@angular/core";
import { MatPaginator, MatSort, MatTableDataSource } from "@angular/material";
import { AlertifyService } from "src/app/_services/alertify.service";
import { NgxSpinnerService } from "ngx-spinner";
import { LessonsService } from "../_services/lessons.service";
import { Subscription } from "rxjs";

@Component({
  selector: "app-mylessons",
  templateUrl: "./mylessons.component.html",
  styleUrls: ["./mylessons.component.css"],
})
export class MylessonsComponent implements OnInit, OnDestroy {
  model: any = {};

  teachingLessons: Teaching_Lessons[];

  dataSource: MatTableDataSource<Teaching_Lessons>;

  displayedColumns = ["Lesson", "Day", "Lessonstart", "Lessonend"];

  lessonSub: Subscription;

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  constructor(
    private alertify: AlertifyService,
    private lessonsService: LessonsService,
    private spinner: NgxSpinnerService
  ) {}

  ngOnDestroy(): void {
    if (this.lessonSub) this.lessonSub.unsubscribe();
  }

  ngOnInit(): void {
    this.getTeachingLessons(
      JSON.parse(localStorage.getItem("decodedToken")).nameid
    );
  }

  applyFilter(filterValue: string): void {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  getTeachingLessons(id: string): void {
    this.spinner.show();
    this.lessonSub = this.lessonsService.getTeachingLessons(id).subscribe(
      (teachingLessons: Teaching_Lessons[]) => {
        this.teachingLessons = teachingLessons;
        this.dataSource = new MatTableDataSource(this.teachingLessons);
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
        this.spinner.hide();
      },
      (error) => {
        this.alertify.error(error);
        this.spinner.hide();
      }
    );
  }
}
