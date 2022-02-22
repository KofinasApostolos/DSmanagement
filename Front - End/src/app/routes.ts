import { HomeComponent } from './home/home.component';
import { UserprofileComponent } from './userprofile/userprofile.component';
import { Routes } from '@angular/router';
import { LessonsComponent } from './lessons/lessons.component';
import { AuthGuard } from './_guards/auth.guard';
import { AdminPanelComponent } from './admin/admin-panel/admin-panel.component';
import { LessonDetailComponent } from './lessons/lesson-detail/lesson-detail.component';
import { BookingComponent } from './lessons/booking/booking.component';
import { CalendarComponent } from './calendar/calendar.component';
import { UserSuggestionComponent } from './user-suggestion/user-suggestion.component';
import { MylessonsComponent } from './mylessons/mylessons.component';
import { MySubscriptionsComponent } from './mysubscriptions/mysubscriptions.component';
import { Auth2 } from './_guards/auth2.guard';
import { Auth3 } from './_guards/auth3.guard';
import { Auth4 } from './_guards/auth4.guard';
import { Auth5 } from './_guards/auth5.guard';

export const appRoutes: Routes = [
  { path: 'home', component: HomeComponent, canActivate: [Auth5] },
  { path: 'lessons', component: LessonsComponent, canActivate: [AuthGuard] },
  { path: 'userprofile', component: UserprofileComponent, canActivate: [Auth4] },
  { path: 'admin', component: AdminPanelComponent, canActivate: [Auth2] },
  { path: 'lesson/:id', component: LessonDetailComponent, canActivate: [AuthGuard] },
  { path: 'payment', component: BookingComponent, canActivate: [AuthGuard] },
  { path: 'calendar', component: CalendarComponent, canActivate: [Auth3] },
  { path: 'suggestion', component: UserSuggestionComponent, canActivate: [AuthGuard] },
  { path: 'subscriptions', component: MySubscriptionsComponent, canActivate: [AuthGuard] },
  { path: 'teachinglessons', component: MylessonsComponent, canActivate: [Auth3] },

  { path: '**', redirectTo: 'home', pathMatch: 'full' }
];
