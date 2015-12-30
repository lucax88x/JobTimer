module JobTimer.Hubs{

/*
   Client interfaces:

 */ 

interface INotificationHubClient {
    starter(): void;
    updateModel(notificationModel: NotificationModel): void;
}

//Promise interface
interface IPromise<T> {
    done(cb: (result: T) => any): IPromise<T>;
    error(cb: (error: any) => any): IPromise<T>;
}

// Data interfaces 
export interface NotificationModel {
    Date: string;
    Action: string;
    Username: string;
}

// Hub interfaces 
interface INotificationHub {
    starter(): IPromise<void>;
    updateModel(notificationModel: NotificationModel): IPromise<void>;
}

// Generated proxies 
export interface INotificationHubProxy {
     server: INotificationHub;
     client: INotificationHubClient;
}
}