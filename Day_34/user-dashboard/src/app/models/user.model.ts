export class UserModel {
  constructor(
    public id: number = 0,
    public username: string = "",
    public email: string = "",
    public firstName: string = "",
    public lastName: string = "",
    public gender: string = "",
    public image: string = "",
    public role: string = "",
    public state: string = ""
  ) {}
}
