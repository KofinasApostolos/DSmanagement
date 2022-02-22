import { FormGroup } from "@angular/forms";
import * as moment from "moment";

export class Helpers {
  private constructor() {}

  public static base64ToBlob(b64Data, contentType = "", sliceSize = 512): Blob {
    b64Data = b64Data.replace(/\s/g, ""); //IE compatibility...
    let byteCharacters = atob(b64Data);
    let byteArrays = [];
    for (let offset = 0; offset < byteCharacters.length; offset += sliceSize) {
      let slice = byteCharacters.slice(offset, offset + sliceSize);
      let byteNumbers = new Array(slice.length);
      for (var i = 0; i < slice.length; i++) {
        byteNumbers[i] = slice.charCodeAt(i);
      }
      let byteArray = new Uint8Array(byteNumbers);
      byteArrays.push(byteArray);
    }
    return new Blob(byteArrays, { type: contentType });
  }

  public static dateConvert(date: string): Date {
    var dateMomentObject = moment(date, "DD/MM/YYYY"); // 1st argument - string, 2nd argument - format
    return dateMomentObject.toDate(); // convert moment.js object to Date object
  }

  public static getErrorMessageTemplateDrivenForm(
    controlName: string,
    errorType: string
  ): string {
    if (errorType === "required") {
      return controlName + " is required";
    } else if (errorType === "minlength" || errorType === "minLength") {
      return controlName + " min length not filled";
    } else if (errorType === "email") {
      return controlName + " email is not valid";
    } else if (errorType === "maxlength" || errorType === "maxLength") {
      return controlName + " max length has been extendeed";
    }
  }

  public static getErrorMessageReactiveForm(
    controlType: string,
    controlName: string,
    controlFormGroup: FormGroup
  ): string {
    if (controlType === "email") {
      return controlFormGroup.controls[controlName].hasError("required")
        ? controlName + " is required"
        : controlFormGroup.controls[controlName].hasError("email")
        ? controlName + " is not valid"
        : "";
    } else if (controlType === "text" || controlType === "password") {
      return controlFormGroup.controls[controlName].hasError("required")
        ? controlName + " is required"
        : controlFormGroup.controls[controlName].hasError("minLength") ||
          controlFormGroup.controls[controlName].hasError("minlength")
        ? "min length not filled"
        : "";
    } else if (
      controlType === "time" ||
      controlType === "select" ||
      controlType === "date"
    ) {
      return controlName + " is required";
    }
  }
}
