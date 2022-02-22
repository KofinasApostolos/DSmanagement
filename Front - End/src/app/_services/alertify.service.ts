import { Injectable } from "@angular/core";
import { Observable, of } from "rxjs";
import Swal from "sweetalert2";

@Injectable({
  providedIn: "root",
})
export class AlertifyService {
  constructor() {}

  success(message: string) {
    //alertify.success(message);
    Swal.fire({
      position: "center",
      icon: "success",
      title: "Success",
      text: message,
      showConfirmButton: false,
      timer: 3000,
    });
  }

  error(message: string) {
    //alertify.error(message);
    Swal.fire({
      position: "center",
      icon: "error",
      title: "Error",
      text: message,
    });
  }

  warning(message: string) {
    // alertify.warning(message);
    Swal.fire({
      position: "center",
      icon: "warning",
      title: "Warning",
      text: message,
    });
  }

  async openConfirmDialog(msg: string, title: string): Promise<any> {
    return Swal.fire({
      title: title,
      text: msg,
      icon: "warning",
      showCancelButton: true,
      confirmButtonColor: "#3f51b5",
      cancelButtonColor: "#f44336",
      confirmButtonText: "Yes",
    }).then((result) => {
      return result;
    });
  }

  toastMessage(message: string) {
    const Toast = Swal.mixin({
      toast: true,
      position: "bottom-end",
      showConfirmButton: false,
      timer: 3000,
      timerProgressBar: true,
      onOpen: (toast) => {
        toast.addEventListener("mouseenter", Swal.stopTimer);
        toast.addEventListener("touchstart", Swal.stopTimer);
        toast.addEventListener("mouseleave", Swal.resumeTimer);
        toast.addEventListener("touchend", Swal.resumeTimer);
      },
    });
    Toast.fire({
      icon: "success",
      title: message,
    });
  }
}
