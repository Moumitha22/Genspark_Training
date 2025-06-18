export class UserAddModel {
  constructor(
    public username: string = "",
    public email: string = "",
    public firstName: string = "",
    public lastName: string = "",
    public gender: string = "",
    public password: string = "",
    public image?: string,
    public company: { title: string } = { title: "" },
    public address: { state: string } = { state: "" }
  ) {}
}
