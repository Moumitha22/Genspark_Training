import { createAction, props } from "@ngrx/store";
import { UserModel } from "../models/User";

export const loadUsers = createAction('[Users] Load Users');
export const loadUsersSuccess = createAction('[Users] Load Users Success',props<{ users: UserModel[] }>());
export const loadUsersFailure = createAction('[Users] Load Users Failure',props<{ error: string }>());
export const addUser = createAction('[Users] Add User',props<{ user: UserModel }>());