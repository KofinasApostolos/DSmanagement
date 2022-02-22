import { BrowserModule } from "@angular/platform-browser";
import { NgModule } from "@angular/core";

import { AppRoutingModule } from "./app-routing.module";
import { AppComponent } from "./app.component";
import { NavComponent } from "./nav/nav.component";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { AuthServiceSys } from "src/app/_services/authSys.service";
import { HttpClientModule } from "@angular/common/http";
import { HomeComponent } from "./home/home.component";
import { AlertifyService } from "src/app/_services/alertify.service";
import { RouterModule } from "@angular/router";
import { appRoutes } from "./routes";
import { LessonsComponent } from "./lessons/lessons.component";
import { AuthGuard } from "./_guards/auth.guard";
import { AdminPanelComponent } from "./admin/admin-panel/admin-panel.component";
import { UserManagementComponent } from "./admin/user-management/user-management.component";
import { AdminService } from "src/app/_services/admin.service";
import { UserModalComponent } from "./admin/user-modal/user-modal.component";
import { UtubeModalComponent } from "./lessons/utube-modal/utube-modal.component";

import {
  MatButtonModule,
  MatTableModule,
  MatPaginatorModule,
  MatSortModule,
  MatFormFieldModule,
  MatInputModule,
  MatSidenavModule,
  MatCheckboxModule,
  MatSliderModule,
  MatTooltipModule,
  MatMenuModule,
  MatIconModule,
  MatTabsModule,
  MatDialogModule,
  MatRadioModule,
  MatSelectModule,
  MatProgressSpinnerModule,
  MatCardModule,
  MAT_DIALOG_DEFAULT_OPTIONS,
  ErrorStateMatcher,
  ShowOnDirtyErrorStateMatcher,
  MatDatepickerModule,
  MatNativeDateModule,
  MAT_DATE_LOCALE,
  MatToolbarModule,
} from "@angular/material";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { LessonCardComponent } from "./lessons/lesson-card/lesson-card.component";
import { LessonDetailComponent } from "./lessons/lesson-detail/lesson-detail.component";
import { MySubscriptionsComponent } from "./mysubscriptions/mysubscriptions.component";
import { BookingComponent } from "./lessons/booking/booking.component";
import { RoleModalComponent } from "./admin/role-modal/role-modal.component";
import { RoleManagementComponent } from "./admin/role-management/role-management.component";
import { CalendarComponent } from "./calendar/calendar.component";
import { adapterFactory } from "angular-calendar/date-adapters/date-fns";
import { CalendarModule, DateAdapter } from "angular-calendar";
import { CalendarHeaderComponent } from "./calendar/calendar-header/calendar-header.component";
import { PasswordStrengthMeterModule } from "angular-password-strength-meter";
import { FileUploadModule } from "ng2-file-upload";
import { ErrorInterceptorProvider } from "src/app/_services/error.interceptor";
import { LessonManagementComponent } from "./admin/lesson-management/lesson-management.component";
import { SubscribeManagementComponent } from "./admin/subscribe-management/subscribe-management.component";
import { UsersubscriptionsManagementComponent } from "./admin/usersubscriptions-management/usersubscriptions-management.component";
import { SubscriptionModalComponent } from "./admin/subscription-modal/subscription-modal.component";
import { LessonModalComponent } from "./admin/lesson-modal/lesson-modal.component";
import { TeachingprogramManagementComponent } from "./admin/teachingprogram-management/teachingprogram-management.component";
import { TeachingprogramModalComponent } from "./admin/teachingprogram-modal/teachingprogram-modal.component";
import { UsersubscriptionsModalComponent } from "./admin/usersubscriptions-modal/usersubscriptions-modal.component";
import { UserprofileComponent } from "./userprofile/userprofile.component";
import { FooterComponent } from "./footer/footer.component";
import { ClosedayComponent } from "./admin/closeday/closeday.component";
import { UserSuggestionComponent } from "./user-suggestion/user-suggestion.component";
import { UsersuggestionManagementComponent } from "./admin/usersuggestion-management/usersuggestion-management.component";
import { IsDecimalDirectiveDirective } from "./_directives/is-decimal-directive.directive";
import { AgmCoreModule } from "@agm/core";
import { NgxSpinnerModule } from "ngx-spinner";
import { MylessonsComponent } from "./mylessons/mylessons.component";
import { Communication } from "./_services/communication.service";
import { FlexLayoutModule } from "@angular/flex-layout";

@NgModule({
  declarations: [
    AppComponent,
    NavComponent,
    HomeComponent,
    LessonsComponent,
    AdminPanelComponent,
    UserManagementComponent,
    UserModalComponent,
    UtubeModalComponent,
    UserprofileComponent,
    LessonCardComponent,
    LessonDetailComponent,
    MySubscriptionsComponent,
    BookingComponent,
    RoleManagementComponent,
    RoleModalComponent,
    CalendarComponent,
    CalendarHeaderComponent,
    LessonManagementComponent,
    SubscribeManagementComponent,
    UsersubscriptionsManagementComponent,
    SubscriptionModalComponent,
    LessonModalComponent,
    TeachingprogramManagementComponent,
    TeachingprogramModalComponent,
    UsersubscriptionsManagementComponent,
    UsersubscriptionsModalComponent,
    UserprofileComponent,
    FooterComponent,
    ClosedayComponent,
    UserSuggestionComponent,
    UsersuggestionManagementComponent,
    IsDecimalDirectiveDirective,
    MylessonsComponent,
  ],
  imports: [
    AgmCoreModule.forRoot({
      apiKey: "AIzaSyCsUSy2yWtzdz92aOw4QaZedPJAw7TIvXo",
    }),
    MatDialogModule,
    FlexLayoutModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatTabsModule,
    BrowserModule,
    HttpClientModule,
    AppRoutingModule,
    MatCheckboxModule,
    MatFormFieldModule,
    MatToolbarModule,
    MatInputModule,
    NgxSpinnerModule,
    ReactiveFormsModule,
    RouterModule.forRoot(appRoutes),
    MatIconModule,
    MatButtonModule,
    FormsModule,
    FileUploadModule,
    MatTableModule,
    MatSidenavModule,
    MatPaginatorModule,
    MatSortModule,
    BrowserAnimationsModule,
    MatProgressSpinnerModule,
    MatTooltipModule,
    MatSliderModule,
    MatMenuModule,
    MatCardModule,
    PasswordStrengthMeterModule,
    MatRadioModule,
    MatSelectModule,
    CalendarModule.forRoot({
      provide: DateAdapter,
      useFactory: adapterFactory,
    }),
  ],
  exports: [MatFormFieldModule, CalendarHeaderComponent, MatSidenavModule],
  providers: [
    AuthServiceSys,
    AlertifyService,
    AuthGuard,
    Communication,
    AdminService,
    ErrorInterceptorProvider,
    {
      provide: MAT_DIALOG_DEFAULT_OPTIONS,
      useValue: { hasBackdrop: false, disableClose: true },
    },
    { provide: ErrorStateMatcher, useClass: ShowOnDirtyErrorStateMatcher },
    { provide: MAT_DATE_LOCALE, useValue: "en-GB" },
  ],
  entryComponents: [
    UserModalComponent,
    TeachingprogramModalComponent,
    UsersubscriptionsModalComponent,
    LessonModalComponent,
    SubscriptionModalComponent,
    RoleModalComponent,
    UtubeModalComponent,
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
