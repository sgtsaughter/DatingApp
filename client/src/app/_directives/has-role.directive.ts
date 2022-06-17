import { take } from 'rxjs/operators';
import { AccountService } from 'src/app/_services/account.service';
import { Directive, Input, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { User } from '../_models/user';

@Directive({
  selector: '[appHasRole]'
})
export class HasRoleDirective implements OnInit {
  @Input() appHasRole: string[];
  user: User;
  constructor(private viewContainerRef: ViewContainerRef, private templateRef: TemplateRef<any>, private accountService: AccountService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => {
      this.user = user;
    })
   }

  ngOnInit(): void {
    // clear the view if no roles
    if (!this.user.roles || this.user == null) {
      this.viewContainerRef.clear();
      return;
    }

    // If the user does have a role that's in the list then we are going to view the element
    if (this.user?.roles.some(r => this.appHasRole.includes(r))) {
      this.viewContainerRef.createEmbeddedView(this.templateRef);
    } else {
      this.viewContainerRef.clear();
    }
  }

}
