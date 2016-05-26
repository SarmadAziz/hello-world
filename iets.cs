Select e.phone, d.name
FROM DEPARTMENT d JOIN d.employee e
Where d.name = 'java'

select e
FROM department d, employee e 
WHERE d.employee = e 

SELECT DISTINCT p
FROM Department d JOIN d.employees e JOIN e.projects p

SELECT DISTINCT p 
FROM Department d, Employee e, Project p 
WHERE d = e.department AND e = p.employee

CriteriaBuilder cb = em.getCriteriaBuilder();
CriteriaQuery<Department> cq = cb.getCriteriaBuilder(Department.class);

Root<Department> dep = cq.from(Department.class);
Root<Employee> emp = cq.from(Employee.class);

cq.select(dep)
  .distinct(true)
  .where(cb.equal(dep, emp.get("department");


