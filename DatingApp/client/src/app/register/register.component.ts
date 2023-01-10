import { Component, Input, OnInit, Output,EventEmitter } from '@angular/core';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  model:any = {};
  @Input() usersFromHomeComponent: any;
  @Output() cancelRegister = new EventEmitter();

  constructor(private accountService: AccountService) { }

  ngOnInit(): void {
  }

  register(){
    console.log(this.model);
    this.accountService.register(this.model).subscribe({
      next: response =>{
        console.log(response);
        this.cancel();
      },
      error: error => console.log(error)    
    })
  }

  cancel(){
    console.log('Cancelled!')
    this.cancelRegister.emit(false);
  }

}
