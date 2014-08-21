; BEGIN - Main Entrypoint
call __MAIN_ENTRYPOINT__ ; Call our main entry point 
						 ; - not strictly necessary but good for setting up esp etc.
		
__MAIN_ENTRYPOINT__:

	push dword ebp
	mov dword ebp, esp

	call %KERNEL_CALL_STATIC_CONSTRUCTORS_METHOD% ; Call the static constructors - this is a macro used by the kernel compiler.
	call %KERNEL_MAIN_METHOD% ; Call our main method - this is a macro used by the kernel compiler.
	
	; We shouldn't ever get to this point! But just in case we do...
	jmp method_System_Void_Kernel_PreReqs_Reset__ ; For now this is our intended behaviour

; END - Main Entrypoint

; BEGIN - GetEIP

; NOTE: Leaves a "dirty stack" on purpose. The aim of this method is for EIP
;		to be on top of the stack after it returns.
GetEIP:
	push dword [ESP]
ret

; END  - GetEIP