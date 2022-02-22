export interface User {
  Id: number;
  Username: string;
  Password: string;
  ConfirmPassword: string;
  Descr: string;
  PasswordSalt: string;
  FirstName: string;
  LastName: string;
  City: string;
  Area: string;
  Street: string;
  Email: string;
  Phonenumber: string;
  Birthdate: Date;
  IsAdmin: number;
  PublicId: string;
  ImageUrl: string;
}
