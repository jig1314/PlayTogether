# Sprint 3 Documentation

## Sprint 3 Forecast & Rationale
In sprint 2, our team was able to complete 9.7 story points of work while each developer was still working through their learning curve with time to spare. The developer found that they were able to spend more time than anticpated on project activities which accounted for the increase in velocity.  
Based on that forecast for sprint 2 and the increase in skill and lessons learned from sprint 2, the team felt comfortable increasing the estimated velocity to 11.5 story points.  
Our rationale is that for the 2 week sprint each developer can dedicate at least 19 hours of work to this project. This is an increase from sprint 2 but is based on actuals from sprint 2 and the commitment from the development team.  
We have 3 developers, working an average of 19 hours over 2 weeks equates to about 115 hours of work. We represent those 115 hours as 11.5 story points.  
We are going to reassess at the end of the sprint if this velocity needs to be decreased, increased or kept the same based on actuals.

## Kanban Board URL
https://dev.azure.com/SWE6733-Team3/PlayTogether/_sprints/taskboard/PlayTogether%20Team/PlayTogether/Sprint%203

## Sprint 3 Burndown Chart
https://dev.azure.com/SWE6733-Team3/PlayTogether/_dashboards/dashboard/b8e2ecb6-bcc4-4b45-ac7b-6944d54fb5c4

## Daily Scrum Reports
Our team recorded our reports from our scrum meetings in an excel workbook. Each worksheet represents each meeting.  
Link to scrum reports for this sprint: https://dev.azure.com/SWE6733-Team3/PlayTogether/_git/PlayTogether?path=%2FSprint%203%2FScrum%20Reports%20(Sprint%203).xlsx&version

## Sprint 3 Mob Programming Session
The developers worked together to develop the BDD test for deleting a user profile.  
Video: https://youtu.be/auyel06D3Cg

## BDD Test Cases
A BDD test was developed for Work Item/Test Case 86 which tests the Delete Profile user story. The Test has been automated, integrated into the CI/CD pipeline and successfully run against each environment.  
More information at: https://dev.azure.com/SWE6733-Team3/PlayTogether/_workitems/edit/86/  
More tests will be developed in future iterations.

## Unit tests
The development team has developed unit tests centered around the vaidation functionality of the Razor Pages using FluentValidation and its testing functionality.  
Unit Tests have been integrated into the CI/CD build pipeline and has successfully passed.  
See example here: https://dev.azure.com/SWE6733-Team3/PlayTogether/_build/results?buildId=58&view=ms.vss-test-web.build-test-results-tab

## Sprint Review/Retrospective
Our team recorded our sprint review in a Word document.  
Here is the link to the document: https://dev.azure.com/SWE6733-Team3/_git/PlayTogether?path=%2FSprint%203%2FSprint%203%20Review.docx
  
Our team recorded our retrospective of Sprint 3 reports in an excel workbook.  
Link to retrospective for this sprint: https://dev.azure.com/SWE6733-Team3/_git/PlayTogether?path=%2FSprint%203%2FSprint%203%20Retrospective.xlsx

## CI/CD Information
Our team's CI/CD process is managed using Azure pipelines and releases housed in our Azure DevOps Project. Unit Tests are run during every build and BDD Tests are run during every release.

### Test Build/Deployment Links
Pipeline: https://dev.azure.com/SWE6733-Team3/PlayTogether/_build?definitionId=3  
Release: https://dev.azure.com/SWE6733-Team3/PlayTogether/_release?_a=releases&view=mine&definitionId=2  

### Production Build/Deployment Links
Pipeline: https://dev.azure.com/SWE6733-Team3/PlayTogether/_build?definitionId=4  
Release: https://dev.azure.com/SWE6733-Team3/PlayTogether/_release?_a=releases&view=mine&definitionId=3

## Working prototype
During this sprint, the test and production environments were stood up and the CI/CD pipeline was developed to delivery updates to these environments.  
Test: https://playtogether-test.azurewebsites.net/  
Production: https://play-together.azurewebsites.net/