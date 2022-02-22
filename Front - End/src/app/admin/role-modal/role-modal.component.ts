import { Component, OnDestroy, OnInit } from "@angular/core";
import { Roles } from "src/app/_models/roles";
import { RolesService } from "src/app/_services/roles.service";
import { AlertifyService } from "src/app/_services/alertify.service";
import { Subject, Subscription } from "rxjs";
import { Helpers } from "src/app/_helpers/helper";

@Component({
  selector: "app-role-modal",
  templateUrl: "./role-modal.component.html",
  styleUrls: ["./role-modal.component.css"],
})
export class RoleModalComponent implements OnInit, OnDestroy {
  model: any = {};

  roles: Roles[];

  public onClose: Subject<boolean>;

  rolesSub: Subscription;

  constructor(
    private roleService: RolesService,
    private alertify: AlertifyService
  ) {}

  ngOnDestroy(): void {
    if (this.rolesSub) this.rolesSub.unsubscribe();
  }

  ngOnInit(): void {
    this.onClose = new Subject();
  }

  public onConfirm(): void {
    this.rolesSub = this.roleService.registerRole(this.model).subscribe(
      (res) => {
        this.onClose.next(true);
        this.alertify.success(res.Message);
      },
      (error) => {
        this.alertify.error(error);
      }
    );
  }

  getErrorMessage(controlName: string, errorType: string) {
    return Helpers.getErrorMessageTemplateDrivenForm(controlName, errorType);
  }

  public onCancel(): void {
    this.onClose.next(false);
  }
}
