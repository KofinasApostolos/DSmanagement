import {
  Component,
  OnInit,
  ViewChild,
  ChangeDetectorRef,
  OnDestroy,
} from "@angular/core";
import {
  MatPaginator,
  MatTableDataSource,
  MatSort,
  MatDialog,
} from "@angular/material";
import { AlertifyService } from "src/app/_services/alertify.service";
import { RolesService } from "src/app/_services/roles.service";
import { Roles } from "src/app/_models/roles";
import { RoleModalComponent } from "../role-modal/role-modal.component";
import { NgxSpinnerService } from "ngx-spinner";
import { Subscription } from "rxjs";

@Component({
  selector: "app-role-management",
  templateUrl: "./role-management.component.html",
  styleUrls: ["./role-management.component.css"],
})
export class RoleManagementComponent implements OnInit, OnDestroy {
  model: any = {};

  private roles: Roles[];

  dataSource: MatTableDataSource<Roles>;

  rolesSub: Subscription;

  displayedColumns = ["RoleCode", "RoleDescr", "actionsColumn"];

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  constructor(
    private alertify: AlertifyService,
    private dialog: MatDialog,
    private rolesService: RolesService,
    private changeDetectorRefs: ChangeDetectorRef,
    private spinner: NgxSpinnerService
  ) {}

  ngOnDestroy(): void {
    if (this.rolesSub) this.rolesSub.unsubscribe();
  }

  ngOnInit(): void {
    this.getRoles();
  }

  refresh(): void {
    this.rolesSub = this.rolesService.getRoles().subscribe((roles: Roles[]) => {
      this.roles = roles;
      this.dataSource = new MatTableDataSource(this.roles);
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
      this.changeDetectorRefs.detectChanges();
    });
  }

  getRoles(): void {
    this.rolesSub = this.rolesService.getRoles().subscribe(
      (roles: Roles[]) => {
        this.roles = roles;
        this.dataSource = new MatTableDataSource(this.roles);
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
      },
      (error) => {
        this.alertify.error(error);
      }
    );
  }

  applyFilter(filterValue: string): void {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  async deleteRole(Id: number): Promise<void> {
    await this.alertify
      .openConfirmDialog(
        "Are you sure you want to delete this record?",
        "Delete"
      )
      .then((res) => {
        if (res.value) {
          this.spinner.show();
          this.rolesSub = this.rolesService.deleteRole(Id).subscribe(
            (res) => {
              this.alertify.success(res.Message);
              this.refresh();
              this.spinner.hide();
            },
            (error) => {
              this.spinner.hide();
              this.alertify.error(error);
            }
          );
        }
      });
  }

  createRole(roles: Roles): void {
    const initialState = {
      roles,
    };
    const dialogRef = this.dialog.open(RoleModalComponent, {
      data: { initialState },
    });
    dialogRef.afterClosed().subscribe((result) => {
      if (result === true) {
        this.spinner.show();
        this.refresh();
        this.spinner.hide();
      }
    });
  }

  async updateRole(RoleCode: number, RoleDescr: string): Promise<void> {
    await this.alertify
      .openConfirmDialog(
        "Are you sure you want to update this record",
        "Update"
      )
      .then((res) => {
        if (res.value) {
          this.spinner.show();
          this.model.RoleCode = RoleCode;
          this.model.RoleDescr = RoleDescr;
          this.rolesSub = this.rolesService.updateRole(this.model).subscribe(
            (res) => {
              //this.roles = roles;
              this.refresh();
              this.alertify.success(res.Message);
              this.spinner.hide();
            },
            (error) => {
              this.spinner.hide();
              this.alertify.error(error);
            }
          );
        }
      });
  }
}
