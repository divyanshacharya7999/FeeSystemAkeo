import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { FeeService } from '../../services/fee.service';

@Component({
  selector: 'app-fee-structure',
  templateUrl: './fee-structure.component.html',
  styleUrl: './fee-structure.component.css'
})
export class FeeStructureComponent {
  model: any = []
  addfee: any = {}
  upfee: any = {}
  delfee:number=0
  
  constructor(private router:Router, private feeService: FeeService){
  this.getfee()
  }

  feetype(){
    this.router.navigate(['feetype'])
  }

  paymentplan(){
    this.router.navigate(['paymentplan'])
  }

  getfee(){
    this.feeService.getAllFee().subscribe(data => {
      console.log(data);
      this.model = data.result.items;
    });
  }

  addFee(){
    this.feeService.addfee(this.addfee).subscribe(response =>{
      alert('Fee Added Successfully');
      window.location.reload();
      this.router.navigate(['feestructure']);
      
    }, error => {
      console.error(error);
      alert('Failed to add Fee');
    });
  }
 

  updatefee(){
    this.feeService.updatefee(this.upfee).subscribe(response =>{
      alert('Fee Updated Successfully');
      window.location.reload();
      this.router.navigate(['feestructure']);
    }, error => {
      console.error(error);
      alert('Failed to update Fee');
    });
  }

  deletefee(delfee:number){
    this.feeService.deletefee(delfee).subscribe(response =>{
      alert('Fee Deleted Successfully');
      window.location.reload();
      this.router.navigate(['feestructure']);
    }, error => {
      console.error(error);
      alert('Failed to Delete Fee');
    });
  }

  

}
