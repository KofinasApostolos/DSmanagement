import {
  Component,
  OnInit,
  ViewChild,
  ChangeDetectorRef,
  EventEmitter,
  Output,
  OnDestroy,
} from "@angular/core";
import { User } from "../../_models/user";
import { AlertifyService } from "src/app/_services/alertify.service";
import { UserService } from "src/app/_services/user.service";
import { UserModalComponent } from "../user-modal/user-modal.component";
import {
  MatDialog,
  MatPaginator,
  MatSort,
  MatTableDataSource,
} from "@angular/material";
import { Roles } from "src/app/_models/roles";
import { RolesService } from "src/app/_services/roles.service";
import { HttpClient, HttpEventType, HttpHeaders } from "@angular/common/http";
import { environment } from "src/environments/environment";
import * as fileSaver from "file-saver";
import { NgxSpinnerService } from "ngx-spinner";
import { Subscription } from "rxjs";
import { Communication } from "src/app/_services/communication.service";
import { Helpers } from "src/app/_helpers/helper";

@Component({
  selector: 'app-user-management"',
  templateUrl: "./user-management.component.html",
  styleUrls: ["./user-management.component.css"],
})
export class UserManagementComponent implements OnInit, OnDestroy {
  private model: any = {};

  private users: User[];
  roles: Roles[];

  userid: string;
  filename: string;

  private baseUrl = environment.apiUrl;

  @Output() photoUrl: string;

  dataSource: MatTableDataSource<User>;

  displayedColumns = [
    "Id",
    "UserPhoto",
    "Username",
    "Password",
    "ConfirmPassword",
    "FirstName",
    "LastName",
    "City",
    "Street",
    "Area",
    "Email",
    "Phonenumber",
    "Birthdate",
    "IsAdmin",
    "Descr",
    "actionsColumn",
  ];

  usersSub: Subscription;
  rolesSub: Subscription;

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  @Output() public onUploadFinished = new EventEmitter();

  constructor(
    private alertify: AlertifyService,
    private userService: UserService,
    private dialog: MatDialog,
    private rolesService: RolesService,
    private changeDetectorRefs: ChangeDetectorRef,
    private http: HttpClient,
    private communicationService: Communication,
    private spinner: NgxSpinnerService
  ) {}

  ngOnDestroy(): void {
    if (this.usersSub) this.usersSub.unsubscribe();
    if (this.rolesSub) this.rolesSub.unsubscribe();
  }

  ngOnInit(): void {
    this.getUsers();
    this.getRoles();
  }

  refresh(): void {
    this.usersSub = this.userService.getUsers().subscribe((users: User[]) => {
      this.users = users;
      this.users.forEach((user) => {
        user.Birthdate = Helpers.dateConvert(user.Birthdate.toString());
      });
      this.dataSource = new MatTableDataSource(this.users);
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
      this.changeDetectorRefs.detectChanges();
    });
  }

  getRoles(): void {
    this.rolesSub = this.rolesService.getRoles().subscribe(
      (roles: Roles[]) => {
        this.roles = roles;
      },
      (error) => {
        this.alertify.error(error);
      }
    );
  }

  uploadImage = (files, id: string): void => {
    this.userid = id;
    let fileToUpload = <File>files[0];
    if (files.length === 0) return;
    const formData = new FormData();
    formData.append("file", fileToUpload, fileToUpload.name);
    this.filename = fileToUpload.name;
    var headers = new HttpHeaders();
    headers = headers.append(
      "Authorization",
      "Bearer " + localStorage.getItem("ecryptedToken")
    );
    this.http
      .post(this.baseUrl + "users/" + this.userid + "/editimage", formData, {
        reportProgress: true,
        observe: "events",
        headers: headers,
      })
      .subscribe(
        (event) => {
          if (event.type === HttpEventType.UploadProgress) {
          } else if (event.type === HttpEventType.Response) {
            this.onUploadFinished.emit(event.body);
            this.refresh();
            const options: {
              params?: any;
            } = {
              params: event.body,
            };
            if (this.userid === JSON.parse(localStorage.getItem("user")).Id) {
              var user = JSON.parse(localStorage.getItem("user"));
              user.ImageUrl = options.params.Url;
              localStorage.setItem("user", JSON.stringify(user));
              this.photoUrl = JSON.parse(localStorage.getItem("user")).ImageUrl;
              this.photoUrl = options.params.Url;
              this.communicationService.emitImageUrl(this.photoUrl);
            }
            this.alertify.success(options.params.Description);
          }
        },
        (error) => {
          this.alertify.error(error);
        }
      );
  };

  exportXLSUsers(): void {
    if (this.users != null) {
      this.spinner.show();
      this.usersSub = this.userService
        .getUsersXLS(
          JSON.parse(localStorage.getItem("decodedToken")).nameid,
          this.users
        )
        .subscribe(
          (response: any) => {
            if (response) {
              var blob = Helpers.base64ToBlob(
                response.Obj.toString(),
                "text/plain"
              );
              fileSaver.saveAs(blob, "users.xlsx");
            }
            this.alertify.success(response.Message);
            this.spinner.hide();
          },
          (error) => {
            this.spinner.hide();
            this.alertify.error(error);
          }
        );
    }
  }

  getUsers(): void {
    this.spinner.show();
    this.usersSub = this.userService.getUsers().subscribe(
      (users: User[]) => {
        this.users = users;
        this.users.forEach((user) => {
          user.Birthdate = Helpers.dateConvert(user.Birthdate.toString());
        });
        this.dataSource = new MatTableDataSource(this.users);
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
        this.spinner.hide();
      },
      (error) => {
        this.spinner.hide();
        this.alertify.error(error);
      }
    );
  }

  applyFilter(filterValue: string): void {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  async deleteUser(Id: number): Promise<void> {
    await this.alertify
      .openConfirmDialog(
        "Are you sure you want to delete this record?",
        "Delete"
      )
      .then((res) => {
        if (res.value) {
          this.spinner.show();
          this.usersSub = this.userService.deleteUser(Id).subscribe(
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

  async updateUser(
    Id: string,
    Username: String,
    FirstName: String,
    LastName: String,
    City: String,
    Street: String,
    Area: string,
    Email: String,
    Phonenumber: String,
    Birthdate: Date,
    IsAdmin: boolean,
    Descr: string,
    Password: string,
    ConfirmPassword: string
  ): Promise<void> {
    await this.alertify
      .openConfirmDialog(
        "Are you sure you want to update this record?",
        "Update"
      )
      .then((res) => {
        if (res.value) {
          this.spinner.show();
          this.model.id = Id;
          this.model.username = Username;
          this.model.firstName = FirstName;
          this.model.lastName = LastName;
          this.model.city = City;
          this.model.street = Street;
          this.model.email = Email;
          this.model.phonenumber = Phonenumber;
          this.model.birthdate = Birthdate;
          this.model.isAdmin = IsAdmin;
          this.model.password = Password;
          this.model.ConfirmPassword = ConfirmPassword;
          this.model.descr = Descr;
          this.model.area = Area;
          this.usersSub = this.userService.updateUser(this.model).subscribe(
            (res) => {
              //this.users = users;
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

  createUser(user: User): void {
    const initialState = {
      user,
    };
    const dialogRef = this.dialog.open(UserModalComponent, {
      data: { initialState },
    });
    dialogRef.afterClosed().subscribe((result) => {
      if (result === true) {
        console.log(result);
        this.spinner.show();
        this.refresh();
        this.spinner.hide();
      }
    });
  }
}
