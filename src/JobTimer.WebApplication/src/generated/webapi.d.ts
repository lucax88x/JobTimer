/// <reference path="constants.ts" />

declare module ViewModels {
	interface BaseViewModel {
		Result: boolean;
		Message: string;
	}
}
declare module ViewModels.Home {
	interface GetShortcutsViewModel extends ViewModels.BaseViewModel {
		Items: Models.Menu.MenuItem[];
	}
	interface SaveShortcutsViewModel extends ViewModels.BaseViewModel {
	}
}
declare module Models.Menu {
	interface MenuItem {
		Id: string;
		Name: string;
		Roles: string[];
		Icon: string;
		Url: string;
		Items: Models.Menu.MenuItem[];
	}
}
declare module BindingModels.Home {
	interface SaveShortcutsBindingModel {
		Shortcuts: string[];
	}
}
declare module ViewModels.Timer {
	interface SaveEnterLunchViewModel extends ViewModels.Timer.SaveViewModel {
	}
	interface SaveViewModel extends ViewModels.Timer.GetStatusViewModel {
		Date: Date;
	}
	interface GetStatusViewModel extends ViewModels.BaseViewModel {
		Status: ViewModels.Timer.TimerStatus;
	}
	interface TimerStatus {
		Enter: boolean;
		EnterLunch: boolean;
		ExitLunch: boolean;
		Exit: boolean;
	}
	interface SaveExitLunchViewModel extends ViewModels.Timer.SaveViewModel {
	}
	interface SaveExitViewModel extends ViewModels.Timer.SaveViewModel {
	}
	interface SaveEnterViewModel extends ViewModels.Timer.SaveViewModel {
	}
}
declare module BindingModels.Timer {
	interface GetStatusBindingModel {
		Date: Date;
	}
	interface SaveExitBindingModel extends BindingModels.Timer.SaveBindingModel {
	}
	interface SaveBindingModel {
		Offset: number;
		Date: Date;
	}
	interface SaveExitLunchBindingModel extends BindingModels.Timer.SaveBindingModel {
	}
	interface SaveEnterLunchBindingModel extends BindingModels.Timer.SaveBindingModel {
	}
	interface SaveEnterBindingModel extends BindingModels.Timer.SaveBindingModel {
	}
}
declare module ViewModels.Master {
	interface GetUserDataViewModel extends ViewModels.Master.UserDataViewModel {
		UserName: string;
		Email: string;
	}
	interface UserDataViewModel extends ViewModels.BaseViewModel {
		TimeOffset: string;
		TimeOffsetType: ViewModels.Master.OffsetTypes;
		HasEstimatedExitTime: boolean;
		EstimatedExitTime: Date;
	}
	interface UpdateUserDataViewModel extends ViewModels.Master.UserDataViewModel {
	}
}
declare module Models {
	interface BaseJson extends Models.ReadJson<number> {
	}
	interface ReadJson<T> {
		ID: T;
	}
}
declare module Models.Role {
	interface RoleSimpleJson {
		Id: string;
		Name: string;
	}
}
declare module ViewModels.Chart {
	interface ChartSerie<T> {
		data: T[];
	}
	interface ChartData<T> {
		name: string;
		series: ViewModels.Chart.ChartSerie<T>[];
	}
	interface ChartViewModel<T> extends ViewModels.BaseViewModel {
		Data: ViewModels.Chart.ChartData<T>;
	}
	interface GetMonthlyLunchTimesViewModel extends ViewModels.Chart.ChartViewModel<number[]> {
	}
	interface GetMonthlyLunchTotalsViewModel extends ViewModels.Chart.ChartViewModel<number> {
	}
	interface GetMonthlyTimesViewModel extends ViewModels.Chart.ChartViewModel<number[]> {
	}
	interface GetMonthlyTotalsViewModel extends ViewModels.Chart.ChartViewModel<number> {
	}
	interface GetWeeklyLunchTimesViewModel extends ViewModels.Chart.ChartViewModel<number[]> {
	}
	interface GetWeeklyLunchTotalsViewModel extends ViewModels.Chart.ChartViewModel<number> {
	}
	interface GetWeeklyTotalsViewModel extends ViewModels.Chart.ChartViewModel<number> {
	}
	interface GetWeeklyTimesViewModel extends ViewModels.Chart.ChartViewModel<number[]> {
	}
}
declare module ViewModels.AdminUser {
	interface GetRolesViewModel extends ViewModels.BaseViewModel {
		Roles: Models.Role.RoleSimpleJson[];
	}
}
declare module ViewModels.Account {
	interface LogoutViewModel extends ViewModels.BaseViewModel {
	}
	interface LoginViewModel extends ViewModels.BaseViewModel {
	}
	interface VerifyExternalLoginViewModel extends ViewModels.BaseViewModel {
	}
	interface RegisterExternalViewModel extends ViewModels.BaseViewModel {
	}
	interface RegisterViewModel extends ViewModels.BaseViewModel {
	}
}
declare module BindingModels.Account {
	interface LoginBindingModel {
		Email: string;
		Password: string;
	}
	interface VerifyExternalLoginBindingModel {
		Provider: string;
		ExternalAccessToken: string;
	}
	interface RegisterBindingModel {
		Email: string;
		Password: string;
		ConfirmPassword: string;
	}
	interface RegisterExternalBindingModel {
		Email: string;
		Provider: string;
		ExternalAccessToken: string;
	}
}
declare module Constants.HttpHeaders {
	interface Request {
	}
}
