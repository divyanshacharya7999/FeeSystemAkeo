import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { FeeService } from '../../../services/fee.service';

@Component({
  selector: 'app-paymentplan',
  templateUrl: './paymentplan.component.html',
  styleUrl: './paymentplan.component.css'
})
export class PaymentplanComponent {
  model: any = []
  delpaymentplan:number=0
  payplan: any = {}
  upplan: any = {}

  constructor(private router:Router, private feeService: FeeService){
    this.getpaymentplan()
    }

  getpaymentplan(){
      this.feeService.getallpaymentplan().subscribe(response =>{
        console.log(response.result);
        this.model = response.result;
    });
  }

  addpaymentplan(){
    this.feeService.addpaymentplan(this.payplan).subscribe(response =>{
      alert('Payment plan Added Successfully');
      window.location.reload();
      this.router.navigate(['paymentplan']);
    }, error => {
      console.error(error);
      alert('Failed to add Fee');
    });
  }

  deletepaymentplan(delpaymentplan:number){
    this.feeService.deletepaymentplan(delpaymentplan).subscribe(response =>{
      alert('Payment Plan Deleted Successfully');
      window.location.reload();
      this.router.navigate(['paymentplan']);
    }, error => {
      console.error(error);
      alert('Failed to Delete Fee');
    });
  }

  updatepaymentplan(){
    this.feeService.updatepaymentplan(this.upplan).subscribe(response => {
      alert('Update added successfully');
      window.location.reload();
     this.router.navigate(['school']);
    }, error => {
      console.error(error);
      alert('Failed to Update Fee Type');
    });
  }

}
