# EasyPay-DeveloperChallenge

This project has the implementation for the given developer challenge. The steps taken are listen below:

* Since the clients file was given from the finance department and an ERP was bought for managing the day to day processes, it was uploaded in the database manually. New clients would be uploaded through the ERP.
* 3 Projects were created:
  * CSVProcessor -> holds the controllers for processing CSV files. If a CSV file holds less than 1000 rows it will be processed instantly. Otherwise a task is scheduled for processing from the worker service.
  * CSVProcessor.WorkerService -> holds the logic for processing CSV files in chunks. When a file is finished processing, it is saved in a local path and is ready to be downloaded from the controller.
  * CSVProcessor.Share -> holds all the interfaces, repositories and models that are used by the two above projects.