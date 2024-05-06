; vim:ft=lisp

(defun get-register-address (register)
  (when (not (is-a-register register))
    (error 'not-a-register))
  (cadr register))

(defun is-a-register (register)
  (and (not (atom register))
      (eq (car register) 'REGISTER)))

(defun shift-into (a b n)
  (logior
    (ash a n)
    (logand b (lognot (ash -1 n)))))

(defun encode (&rest byte-pairs)
  (let ((instruction 0))
    (dolist (pair byte-pairs)
            (setq instruction (shift-into instruction (car pair) (cadr pair))))
    instruction))
    
(defun assemble-load (args)
  (let ((register (get-register-address (car args)))
        (value (cadr args)))
    (cond ((atom value)
           (encode '(#x01 8)
                   (list register 8)
                   (list value 16)))
          ((is-a-register value)
           (encode '(#x02 8)
                   (list register 8)
                   (list (get-register-address value) 8)
                   '(0 8))))))

(defun assemble-instruction (expr)
  (let ((operation (car expr))
        (args (cdr expr)))
    (cond ((eq operation '@LOAD)
           (assemble-load args)))))
           

(defun assemble (&rest exprs)
  (dolist (expr exprs)
    (assemble-instruction expr)))

(defclass load-instruction ()
  ((register
     :initarg :register
     :accessor register)))

(format t "~X" (assemble-instruction '(@load (register 1) (register 5))))
