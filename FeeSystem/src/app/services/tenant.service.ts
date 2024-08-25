export enum TenantAvailabilityState {
  _1 = 1,
  _2 = 2,
  _3 = 3,
}

export class AppTenantAvailabilityState {
  static Available: number = TenantAvailabilityState._1;
  static InActive: number = TenantAvailabilityState._2;
  static NotFound: number = TenantAvailabilityState._3;
}

